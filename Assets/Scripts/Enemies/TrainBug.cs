using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrainBug : MonoBehaviour
{
    private enum EStates
    {
        charging,
        vulnerable,
        dead,
    }
    private EStates m_state = EStates.charging;

    [SerializeField] private Transform m_targetTransform;
    private Vector3 m_startPosition;

    [SerializeField] private GameObject m_meshParent = null;
    [SerializeField] private MeshRenderer m_meshRenderer = null;
    private AudioSource m_chargingSound;
    private GlobalEnemySettings m_settings;
    private Rigidbody m_rigidbody;
    private StunnedStars m_stunnedStars = null;
    private Vector3 m_stunnedLocation = Vector3.zero;

    private float m_vulnerableTimer = 0.0f;

    private void Awake()
    {
        m_startPosition = transform.position;
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");
        m_rigidbody = GetComponent<Rigidbody>();
        m_chargingSound = GetComponent<AudioSource>();
        m_chargingSound.Play();
        m_stunnedStars = GetComponentInChildren<StunnedStars>();
    }

    private void FixedUpdate()
    {
        // Check state //

        switch (m_state)
        {
            case EStates.vulnerable:
                {
                    if (IsTimerFinished())
                    {
                        m_chargingSound.Play();
                        m_state = EStates.charging;
                        Flip(false);
                        transform.DOMove(m_stunnedLocation, 0.1f);
                    }
                    break;
                }

            case EStates.charging:
                {
                    MoveForward();
                    break;
                }
        }
    }

    // Called when hit player or stationary chunk
    private void OnCollisionEnter(Collision collision)
    {
        // Hit Chunk
        Chunk chunk = collision.collider.GetComponentInParent<Chunk>();
        if (chunk)
        {
            // Check for stun
            if (m_state == EStates.charging && chunk.m_currentEffect == EChunkEffect.none)
            {
                // Stunned
                Stun();
            }

            Destroy(chunk.gameObject);

            return;
        }

        // Hit Player
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player && m_state == EStates.charging)
        {
            player.GetComponent<HealthComponent>().Health -= 1;
            player.KnockBack(player.transform.position - transform.position);
        }
    }

    // Called when hit moving chunk
    private void OnTriggerEnter(Collider other)
    {
        // Hit chunk
        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk)
        {
            // Check for stun
            if (m_state == EStates.charging && chunk.m_currentEffect == EChunkEffect.none)
            {
                // Stunned
                Stun();
            }

            // Check for dead
            else if (m_state == EStates.vulnerable && chunk.m_currentEffect == EChunkEffect.water)
            {
                // Dead
                Dead();
            }

            Destroy(chunk.gameObject);
        }
    }

    // Flips the grub either upside down or right side up
    private void Flip(bool _upsideDown)
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        if (_upsideDown)
        {
            rotation.z += 180.0f;
        }
        m_meshParent.transform.DORotate(rotation, 0.5f);
    }

    // Checks the timer for the bug being vulnerable
    private bool IsTimerFinished()
    {
        if (m_vulnerableTimer <= 0.0f)
        {
            return true;
        }

        m_vulnerableTimer -= Time.fixedDeltaTime;

        return false;
    }

    // Moves the enemy forward - if it reaches its objective, teleport
    private void MoveForward()
    {
        m_rigidbody.AddForce(transform.forward * m_settings.m_trainSpeed, ForceMode.Impulse);

        // If reached objective
        float sqrRad = 0.5f * 0.5f;
        float sqrMag = (m_targetTransform.position - transform.position).sqrMagnitude;
        if (sqrMag <= sqrRad)
        {
            // Teleport
            transform.position = m_startPosition;
        }
    }

    // Stuns the bug
    private void Stun()
    {
        // Snap to tile
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0.0f, 0.1f, 0.0f), Vector3.down, out hit, 0.5f))
        {
            Debug.Log("Raycast hit: " + hit.transform.gameObject.name);

            if (hit.transform.GetComponent<Tile>() || hit.transform.parent.GetComponent<Tile>())
            {
                Vector3 newPos = transform.position;
                newPos.x = hit.transform.position.x;
                newPos.z = hit.transform.position.z;
                transform.DOMove(newPos, 0.2f);
            }
        }

        m_stunnedLocation = transform.position;

        MessageBus.TriggerEvent(EMessageType.chunkDestroyed);
        m_chargingSound.Stop();
        m_rigidbody.velocity = Vector3.zero;
        Flip(true);
        m_vulnerableTimer = m_settings.m_trainVulernableTime;
        m_state = EStates.vulnerable;
        m_stunnedStars.Init(m_settings.m_trainVulernableTime);
    }

    // Kills the bug
    private void Dead()
    {
        MessageBus.TriggerEvent(EMessageType.grubKilled);

        m_state = EStates.dead;

        m_meshParent.transform.DOScale(0.1f, 0.2f).OnComplete(() => Destroy(transform.parent.gameObject));
    }
}
