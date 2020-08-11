using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public enum EChunkEffect
{
    none,
    water,
    fire,
    mirage
}

[RequireComponent(typeof(Rigidbody))]
public class Chunk : MonoBehaviour
{
    // Public variables
    [HideInInspector] public EChunkEffect m_currentEffect = EChunkEffect.none;
    [HideInInspector] public EChunkEffect CurrentEffect
    {
        get { return m_currentEffect; }
        set
        {
            m_currentEffect = value;
        }
    }
    [HideInInspector] public EChunkType m_chunkType = EChunkType.none;
    [HideInInspector] public bool m_isRaised = false;

    // Serialized variables
    [SerializeField] private ChunkSettings m_settings;
    [SerializeField] private bool m_startOverride = false;
    [SerializeField] private GameObject[] m_meshObjects = new GameObject[3];

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
    private void OnEnable()
    {
        ChunkManager.AddChunk(this);
    }
    private void OnDisable()
    {
        ChunkManager.RemoveChunk(this);
    }

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
    private void Start()
    {
        transform.parent = RoomManager.Instance.GetActiveRoom().transform;
    }

    private void OnApplicationQuit()
    {
        m_globalSettings.m_isQuitting = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Chunk trigger hit " + other.gameObject.name);

        // If collider has hit any of these, return
        // Should be refactored further
        if (CollisionHasComponent<Hurtbox>(other, null) ||
            CollisionHasComponent<Projectile>(other, null) ||
            CollisionHasComponent<PressurePlate>(other, null) ||
            CollisionHasComponent<MirageBullet>(other, null))
        {
            return;
        }

        // Collides with Toad Boss
        ToadBoss boss = other.GetComponent<ToadBoss>();
        if (boss)
        {
            boss.OnHit();
            Destroy(this.gameObject);
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

        CobraHealth cobra = other.GetComponent<CobraHealth>();
        if (cobra)
        {
            CobraHealth.Damage();
            Destroy(gameObject);
            return;
        }

        CobraMirageClone clone = other.GetComponent<CobraMirageClone>();
        if (clone)
        {
            clone.Damage();
            Destroy(gameObject);
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
            if (_action != null)
            {
                _action.Invoke();
            }
            return true;
        }

        // Collider does not have the component
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
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

    // Called when this chunk collides with a centipede segment
    private void HitCentipedeSegment(Collider _collider)
    {
        if (CentipedeTrainAttack.s_charging && m_currentEffect == EChunkEffect.none && !CentipedeTrainAttack.s_stunned)
        {
            _collider.GetComponentInParent<CentipedeTrainAttack>().HitByChunk();
            Destroy(this.gameObject);
            return;
        }

        if (m_currentEffect == EChunkEffect.water)
        {
            _collider.GetComponent<CentipedeSegmentMover>().Damaged();
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

    public void OnDeath()
    {
        switch (m_currentEffect)
        {
            case EChunkEffect.water:
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


        Destroy(gameObject);
    }

    // Pushes the chunk in a specific direction when hit by the player
    public bool Hit(Vector3 _hitVec, EChunkEffect _effect)
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
        if (cardinal.x >= 0.9f)
        {
            m_posXCollider.enabled = true;
        }
        else if (cardinal.x <= -0.9f)
        {
            m_negXCollider.enabled = true;
        }
        else if (cardinal.z >= 0.9f)
        {
            m_posZCollider.enabled = true;
        }
        else
        {
            m_negZCollider.enabled = true;
        }

        // Change effect
        UpdateEffect(_effect, _hitVec);

        m_rigidBody.AddForce(_hitVec, ForceMode.Impulse);

        MessageBus.TriggerEvent(EMessageType.chunkHit);

        return true;
    }

    // Checks if the chunk is against a wall using a raycast
    private bool IsAgainstWall(Vector3 _hitVec)
    {
        // Start position - almost the bottom of the chunk
        Vector3 checkPosition = transform.position + _hitVec.normalized;

        // Raycast in the direction of the hit vector for half a chunk's length
        //if (Physics.Raycast(checkPosition, _hitVec, out hit, m_globalSettings.m_wallCheckDistance, m_globalSettings.m_wallLayers))
        Collider[] hits = Physics.OverlapBox(checkPosition, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, m_globalSettings.m_wallLayers);
        if (hits.Length > 0)
        {
            // Hit something

            // Ignore sand
            foreach (Collider i in hits)
            {
                SandBlock sand = i.transform.GetComponent<SandBlock>();
                if (!sand || (sand && sand.m_isGlass)) 
                { 
                    return true;
                }
            }
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
            case EChunkEffect.water:
                {
                    OnDeath();
                    break;
                }

            case EChunkEffect.fire:
                {
                    OnDeath();
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
    public void SnapToTongue(Vector3 _tonguePos, Vector3 _frogPos)
    {
        m_rigidBody.velocity = Vector3.zero; // Reset velocity

        // Disable all collision
        DisableAllColliders();
        m_mainCollider.enabled = false;

        // Tween to tongue
        transform.DOMove(_tonguePos, 0.2f);
        Vector3 distance = _frogPos - transform.position;
        distance.y = 0.0f;
        float tweenTime = distance.magnitude / 3.0f;
        transform.DOMove(_frogPos, tweenTime);
    }

    // Changes the current effect of the chunk and updates the mesh accordingly
    private void UpdateEffect(EChunkEffect _effect, Vector3 _hitDir)
    {
        if (m_chunkType == EChunkType.carapace)
        {
            return;
        }

        m_currentEffect = _effect;

        // Changes the active mesh depending on the infused power
        for (int i = 0; i < m_meshObjects.Length; i++)
        {
            m_meshObjects[i].SetActive(i == (int)_effect);
        }

        if (_effect != EChunkEffect.none)
        {
            m_meshObjects[(int)_effect].transform.rotation = Quaternion.LookRotation(_hitDir);
        }
    }
}
