using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public enum eChunkEffect
{
    none,
    water,
    fire
}

[RequireComponent(typeof(Rigidbody))]
public class Chunk : MonoBehaviour
{
    // Public variables
    [HideInInspector] public eChunkEffect m_currentEffect = eChunkEffect.none;
    [HideInInspector] public eChunkEffect CurrentEffect
    {
        get { return m_currentEffect; }
        set
        {
            m_waterParticles.SetActive(false);
            m_fireParticles.SetActive(false);

            if (value == eChunkEffect.water) { m_waterParticles.SetActive(true); }
            if (value == eChunkEffect.fire) { m_fireParticles.SetActive(true); }

            m_currentEffect = value;
        }
    }
    [HideInInspector] public eChunkType m_chunkType = eChunkType.none;
    [HideInInspector] public bool m_isRaised = false;

    // Serialized variables
    [SerializeField] private ChunkSettings m_settings;
    [SerializeField] private bool m_startOverride = false;
    [SerializeField] private GameObject m_waterParticles;
    [SerializeField] private GameObject m_fireParticles;

    // Private variables
    private Rigidbody m_rigidBody;
    private Vector3 m_spawnPos;
    GlobalChunkSettings m_globalSettings;
    private HealthComponent m_healthComp;
    private Vector3 m_prevVelocity = Vector3.zero;

    // Hitboxes
    [SerializeField] private Collider m_posXCollider;
    [SerializeField] private Collider m_negXCollider;
    [SerializeField] private Collider m_posZCollider;
    [SerializeField] private Collider m_negZCollider;

    [SerializeField] private Collider m_mainCollider;

    // Chunks automatically added and removed to chunk manager over lifetime
    private void OnEnable() => ChunkManager.AddChunk(this);
    private void OnDisable() => ChunkManager.RemoveChunk(this);

    private void Awake()
    {
        // Set values
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.isKinematic = true;

        // Setup health component
        m_healthComp = GetComponent<HealthComponent>();
        m_healthComp.Init(m_settings.m_maxHealth, m_settings.m_maxHealth, OnHurt, null, OnDeath);

        m_spawnPos = transform.position;

        m_globalSettings = Resources.Load<GlobalChunkSettings>("ScriptableObjects/GlobalChunkSettings");

        if (m_startOverride)
        {
            m_isRaised = true;
        }
    }

    // For the initial chunk
    private void Start() => transform.parent = RoomManager.Instance.GetActiveRoom().transform;

    private void OnApplicationQuit()
    {
        m_globalSettings.m_isQuitting = true;
    }
    
    private void OnDestroy()
    {
        transform.DOKill();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.gameObject.name);

        // If collider has hit any of these, return
        // Should be refactored further
        if (CollisionHasComponent<Hurtbox>(other, null) ||
            CollisionHasComponent<Projectile>(other, null) ||
            CollisionHasComponent<PressurePlate>(other, null))
        {
            return;
        }
        
        // Collides with Toad Boss
        if (CollisionHasComponent<ToadBoss>(other, ToadBossHit))
        {
            return;
        }
        
        // Chunk hit a centipede segment
        if (CollisionHasComponent<CentipedeSegmentMover>(other, null))
        {
            // If the chunk's trigger is hitting a collider, check if the chunk has hit the centipede boss while charging
            if (!other.isTrigger)
            {
                HitCentipedeSegment(other);
            }

            return;
        }

        // Destroys the lava trail if hit by this chunk
        CentipedeLavaTrail trail = other.GetComponent<CentipedeLavaTrail>();
        if (trail)
        {
            Destroy(trail.gameObject);
            return;
        }

        // Chunk hit a fire bug enemy
        FireBug fireBug = other.GetComponent<FireBug>();
        if (fireBug && !other.isTrigger)
        {
            fireBug.Hit(m_currentEffect);
            Destroy(this.gameObject);
            return; 
        }

        // If the other is a trigger, don't look into snapping
        if (other.isTrigger)
        {
            return;
        }

        // Did not hit ground or player
        if (other.tag != "Ground" && other.tag != "Player" && other.tag != "Lava")
        {
            if (IsAgainstWall(m_prevVelocity.normalized))
            {
                HitWall();
            }
        }
    }

    // Checks if the given collider has the component given - if true, action is invoked
    private bool CollisionHasComponent<T>(Collider _collider, System.Action _action)
    {
        T component = _collider.GetComponent<T>();

        // Collider has the component
        if (component != null)
        {
            _action.Invoke();
            return true;
        }

        // Collider does not have the component
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);

        ToadBoss boss = collision.gameObject.GetComponent<ToadBoss>();
        if (boss)
        {
            boss.OnHit();
            Destroy(this.gameObject);
            return;
        }
    }

    private void FixedUpdate()
    {
        m_prevVelocity = m_rigidBody.velocity;
    }

    // Called when this chunk collides with the toad boss
    private void ToadBossHit()
    {
        boss.OnHit();
        Destroy(this.gameObject);
    }

    // Called when this chunk collides with a centipede segment
    private void HitCentipedeSegment(Collider _colldier)
    {
        if (CentipedeTrainAttack.m_charging && m_currentEffect == eChunkEffect.none && !CentipedeTrainAttack.m_stunned)
        {
            _colldier.GetComponentInParent<CentipedeTrainAttack>().HitByChunk();
            Destroy(this.gameObject);
            return;
        }

        if (m_currentEffect == eChunkEffect.water)
        {
            _colldier.GetComponent<CentipedeSegmentMover>().Damaged();
        }
        Destroy(this.gameObject);
    }

    // Called when chunk is to be raised
    public void RaiseChunk()
    {
        StartCoroutine(Raise());
    }

    // Raises the chunk
    private IEnumerator Raise()
    {
        // Spawn particles
        // Move particles upwards

        transform.DOKill();
        transform.DOMoveY(m_spawnPos.y + m_globalSettings.m_raiseAmount, m_globalSettings.m_raiseTime).SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(m_globalSettings.m_raiseTime);

        m_isRaised = true;
    }

    private void OnHurt()
    {
        MessageBus.TriggerEvent(EMessageType.chunkDamaged);
    }

    private void OnDeath()
    {
        switch (m_currentEffect)
        {
            case eChunkEffect.water:
                {
                    MessageBus.TriggerEvent(EMessageType.waterChunkDestroyed);
                    break;
                }

            default:
                {
                    MessageBus.TriggerEvent(EMessageType.chunkDestroyed);
                    break;
                }
        }


        Destroy(this.gameObject);
    }

    // Pushes the chunk in a specific direction when hit by the player
    public bool Hit(Vector3 _hitVec)
    {
        if (!m_isRaised) { return false; }

        if (IsAgainstWall(_hitVec))
        {
            // Play sound
            m_healthComp.Health -= 1;
            return false;
        }

        Detach();

        // Enable the correct directional collider
        Vector3 cardinal = _hitVec.normalized;
        if (cardinal.x >= 0.9f) { m_posXCollider.enabled = true; }
        else if (cardinal.x <= -0.9f) { m_negXCollider.enabled = true; }
        else if (cardinal.z >= 0.9f) { m_posZCollider.enabled = true; }
        else { m_negZCollider.enabled = true; } // else if (cardinal.z <= -1.0f)

        m_rigidBody.AddForce(_hitVec, ForceMode.Impulse);

        MessageBus.TriggerEvent(EMessageType.chunkHit);

        return true;
    }

    // Checks if the chunk is against a wall using a raycast
    private bool IsAgainstWall(Vector3 _hitVec)
    {
        // Start position - almost the bottom of the chunk
        Vector3 checkPosition = transform.position;
        checkPosition.y -= m_globalSettings.m_chunkHeight * 0.4f;

        // Raycast in the direction of the hit vector for half a chunk's length
        RaycastHit hit;
        if (Physics.Raycast(checkPosition, _hitVec, out hit, m_globalSettings.m_wallCheckDistance, m_globalSettings.m_wallLayers))
        {
            // Hit something

            // Ignore sand
            SandBlock sand = hit.transform.GetComponent<SandBlock>();
            if (sand && !sand.m_isGlass) { return false; }

            return true;
        }

        return false;
    }

    // Makes the chunk moveable
    public void Detach()
    {
        m_rigidBody.isKinematic = false;
        m_rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        m_mainCollider.enabled = false;
    }

    // Decides what to do when the chunk hits a wall
    private void HitWall()
    {
        switch (m_currentEffect)
        {
            case eChunkEffect.water:
                {
                    OnDeath();
                    break;
                }

            case eChunkEffect.fire:
                {
                    CurrentEffect = eChunkEffect.none;
                    MessageBus.TriggerEvent(EMessageType.chunkHitWall);
                    SnapChunk();
                    break;
                }

            default:
                {
                    MessageBus.TriggerEvent(EMessageType.chunkHitWall);
                    ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.small);
                    SnapChunk();
                    break;
                }
        }
    }

    // Snaps a chunk to the nearest grid tile
    private void SnapChunk()
    {
        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        m_rigidBody.isKinematic = true;

        // Change colliders
        DisableAllColliders();
        m_mainCollider.enabled = true;

        // Find nearest grid tile
        Tile nearest = Grid.FindClosestTileAny(transform.position);

        // Snap to the nearest grid tile
        if (nearest)
        {
            Vector3 newPos = nearest.transform.position;
            newPos.y = transform.position.y;
            transform.position = newPos;
        }
        else { Debug.LogError("Unable to find nearest tile to snap to"); }
    }

    public void OnStuckToTongue()
    {
        DisableAllColliders();
        m_mainCollider.enabled = false;
        m_rigidBody.velocity = Vector3.zero;
    }
        
    private void DisableAllColliders()
    {
        // Change colliders
        m_posXCollider.enabled = false;
        m_negXCollider.enabled = false;
        m_posZCollider.enabled = false;
        m_negZCollider.enabled = false;
    }

    // Snaps the chunk to a given position
    public void SnapToTongue(Vector3 _tonguePos)
    {
        m_rigidBody.velocity = Vector3.zero;

        DisableAllColliders();
        m_mainCollider.enabled = false;

        transform.DOMove(_tonguePos, 0.2f);
    }
}
