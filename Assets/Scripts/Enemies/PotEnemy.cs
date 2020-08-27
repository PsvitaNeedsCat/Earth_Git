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
    }

    private EStates m_state = EStates.floating;
    private GlobalEnemySettings m_settings;
    private Player m_playerRef = null;
    private Rigidbody m_rigidbody = null;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");
        m_playerRef = FindObjectOfType<Player>();
        Debug.Assert(m_playerRef, "Couldn't find player in pot enemy");
        m_rigidbody = GetComponent<Rigidbody>();

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
                        m_state = EStates.chasing;
                    }
                    break;
                }

            // Follow the player in the air until the player is reached
            case EStates.chasing:
                {
                    MoveTowardsPlayer();
                    if (PlayerIsWithinRadius(m_settings.m_potAttackRadius))
                    {
                        m_rigidbody.velocity = Vector3.zero;
                        StartCoroutine(SlamDown());
                        m_state = EStates.attacking;
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
        yield return new WaitForSeconds(0.25f);

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
}
