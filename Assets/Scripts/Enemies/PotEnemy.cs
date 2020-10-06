using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PotEnemy : MonoBehaviour
{
    private enum EStates
    {
        floating,
        chasing,
        attacking,

        spotting,
    }

    private EStates m_state = EStates.floating;
    private GlobalEnemySettings m_settings;
    private Player m_playerRef = null;
    private Rigidbody m_rigidbody = null;
    private Animator m_animator = null;

    [SerializeField] private GameObject m_spottedEffect = null;
    [SerializeField] private GameObject m_windupEffect = null;

    private void OnEnable()
    {
        MoveAboveGround();

        m_state = EStates.floating;
    }

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");
        m_playerRef = FindObjectOfType<Player>();
        Debug.Assert(m_playerRef, "Couldn't find player in pot enemy");
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponentInChildren<Animator>();

        MoveAboveGround();
    }

    private void FixedUpdate()
    {
        switch (m_state)
        {
            // Float stationary and wait for player
            case EStates.floating:
                {
                    if (PlayerIsWithinRadius(m_settings.m_potCheckRadius))
                    {
                        m_state = EStates.spotting;

                        StartCoroutine(SpawnSpottedEffect());
                    }
                    break;
                }

            // Follow the player in the air until the player is reached
            case EStates.chasing:
                {
                    MoveTowardsPlayer();
                    if (PlayerIsWithinRadius(m_settings.m_potAttackRadius))
                    {
                        m_state = EStates.attacking;

                        m_rigidbody.velocity = Vector3.zero;

                        StartCoroutine(SpawnWindupEffect());
                    }
                    break;
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only check if in attacking state
        if (m_state != EStates.attacking)
        {
            return;
        }

        // Hit the player
        if (other.GetComponent<Player>())
        {
            other.GetComponent<HealthComponent>().Health -= 1;
        }

        BreakPot();
    }

    // Kills the enemy and breaks the pot
    private void BreakPot()
    {
        MessageBus.TriggerEvent(EMessageType.potDestroyed);
        StopAllCoroutines();
        Destroy(gameObject);

        EffectsManager.SpawnEffect(EffectsManager.EEffectType.potBreak, transform.position, Quaternion.identity, Vector3.one, 1.0f);
    }

    // Checks the distance between the player and the enemy
    private bool PlayerIsWithinRadius(float _radius)
    {
        // Radius squared
        float sqrCheckRadius = _radius * _radius;

        // Distance between player and enemy
        Vector3 checkDist = m_playerRef.transform.position - transform.position;
        checkDist.y = 0.0f;

        // Get square magnitude
        float sqrMagnitude = checkDist.sqrMagnitude;

        return sqrMagnitude <= sqrCheckRadius;
    }

    // Faces the player and moves towards them
    private void MoveTowardsPlayer()
    {
        // Adjust height
        //MoveAboveGround();

        // Face the player
        Vector3 view = m_playerRef.transform.position - transform.position;
        view.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(view);

        // Move forward
        m_rigidbody.AddForce(transform.forward * m_settings.m_potEnemySpeed, ForceMode.Impulse);
    }

    // Slams the pot downstairs quite fast
    private IEnumerator SlamDown()
    {
        m_animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            m_rigidbody.AddForce(Vector3.down * m_settings.m_potSlamSpeed, ForceMode.Impulse);

            yield return null;
        }
    }

    // Raycasts the floor and hovers above it
    private void MoveAboveGround()
    {
        // Raycast
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, m_settings.m_potHoverLayerMask))
        {
            // Move pot enemy
            float newY = hit.transform.position.y + m_settings.m_potHoverHeight;

            // Adjust transform
            Vector3 newPos = transform.position;
            newPos.y = newY;
            transform.position = newPos;
        }
    }

    // Spawns the spotted effect for a set amount of time
    private IEnumerator SpawnSpottedEffect()
    {
        m_spottedEffect.SetActive(true);

        MessageBus.TriggerEvent(EMessageType.playerSpotted);

        yield return new WaitForSeconds(0.5f);

        m_state = EStates.chasing;

        m_spottedEffect.SetActive(false);
    }

    // Spawns the winding up effect for a set amount of time
    private IEnumerator SpawnWindupEffect()
    {
        m_windupEffect.SetActive(true);

        MessageBus.TriggerEvent(EMessageType.chargingUp);

        yield return new WaitForSeconds(0.5f);

        m_windupEffect.SetActive(false);

        StartCoroutine(SlamDown());
    }
}
