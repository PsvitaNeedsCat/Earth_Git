﻿using System.Collections;
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
    [HideInInspector] public bool m_isBeingDestoyed = false;

    // Serialized variables
    [SerializeField] private ChunkSettings m_settings;
    [SerializeField] private bool m_startOverride = false;
    [SerializeField] private GameObject[] m_meshObjects = new GameObject[3];

    public MeshRenderer m_renderer;

    // Private variables
    private Rigidbody m_rigidBody;
    private Vector3 m_spawnPos;
    private GlobalChunkSettings m_globalSettings;
    private HealthComponent m_healthComp;
    private Vector3 m_prevVelocity = Vector3.zero;
    private Vector3 m_hitDirection = Vector3.zero;

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
        m_healthComp.m_type = HealthComponent.EHealthType.chunk;

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
        // If collider has hit any of these, return
        // Should be refactored further
        if (CollisionHasComponent<Hurtbox>(other, null) ||
            CollisionHasComponent<Projectile>(other, null) ||
            CollisionHasComponent<PressurePlate>(other, null) ||
            CollisionHasComponent<MirageBullet>(other, null) ||
            CollisionHasComponent<ChunkKillBox>(other, null))
        {
            return;
        }

        // Collides with Toad Boss
        ToadBoss boss = other.GetComponent<ToadBoss>();
        if (boss)
        {
            boss.OnHit();
            OnDeath();
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
            OnDeath();
            return;
        }

        CobraMirageClone clone = other.GetComponent<CobraMirageClone>();
        if (clone)
        {
            clone.Damage();
            OnDeath();
            return;
        }

        // Chunk hit a fire bug enemy
        FireBug fireBug = other.GetComponent<FireBug>();
        if (fireBug && !other.isTrigger)
        {
            fireBug.Hit(m_currentEffect);
            OnDeath();
            return;
        }

        CentipedeShield centipedeShield = other.GetComponent<CentipedeShield>();
        if (centipedeShield)
        {
            OnDeath();
            centipedeShield.HitChunk();
            return;
        }
        
        if (m_currentEffect == EChunkEffect.water)
        {
            Torch torch = other.GetComponent<Torch>();
            if (torch)
            {
                torch.AttemptToDeactivate();
                OnDeath();
                return;
            }
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
        CentipedeHead head = _collider.GetComponent<CentipedeHead>();
        if (head)
        {
            if (CentipedeTrainAttack.s_charging && m_currentEffect == EChunkEffect.none && !CentipedeTrainAttack.s_stunned && !head.m_health.IsSectionDamaged(CentipedeHealth.ESegmentType.head))
            {
                _collider.GetComponentInParent<CentipedeTrainAttack>().HitByChunk();
                m_healthComp.Health = 0;
                return;
            }
        }

        if (m_currentEffect == EChunkEffect.water)
        {
            _collider.GetComponent<CentipedeSegmentMover>().Damaged();
        }

        m_healthComp.Health = 0;
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

        // Switch to cracked texture
        if (m_healthComp.Health == 1)
        {
            m_meshObjects[0].GetComponent<MeshRenderer>().material.SetFloat("_Crack", 1.0f);
        }
    }

    public void OnDeath()
    {
        m_isBeingDestoyed = true;

        switch (m_currentEffect)
        {
            case EChunkEffect.water:
                {
                    MessageBus.TriggerEvent(EMessageType.waterChunkDestroyed);
                    Quaternion rot = Quaternion.LookRotation(m_hitDirection, Vector3.up);
                    EffectsManager.SpawnEffect(EffectsManager.EEffectType.waveDestroyed, transform.position, rot, Vector3.one, 1.0f);
                    break;
                }

            case EChunkEffect.fire:
                {
                    MessageBus.TriggerEvent(EMessageType.fieryExplosion);
                    break;
                }

            default:
                {
                    EffectsManager.SpawnEffect(EffectsManager.EEffectType.rockBreak, transform.position, Quaternion.identity, Vector3.one, 1.0f, m_renderer.material);
                    MessageBus.TriggerEvent(EMessageType.chunkDestroyed);
                    break;
                }
        }

        Destroy(gameObject);
    }

    // Pushes the chunk in a specific direction when hit by the player
    public bool Hit(Vector3 _hitVec, EChunkEffect _effect)
    {
        if (!m_isRaised)
        {
            return false;
        }

        if (m_chunkType != EChunkType.carapace && _effect == EChunkEffect.fire)
        {
            m_currentEffect = _effect;
            FieryExplosion(_hitVec);
            return true;
        }

        if ((_effect != EChunkEffect.water || m_chunkType == EChunkType.carapace) && IsAgainstWall(_hitVec))
        {
            // Play sound
            m_healthComp.Health -= 1;

            // Spawn effect
            Vector3 hitDir = _hitVec.normalized;
            Vector3 effectPos = transform.position + -hitDir * 0.5f;
            EffectsManager.EEffectType effectType = EffectsManager.EEffectType.rockDamage;
            Quaternion effectRot = Quaternion.LookRotation(-hitDir);
            EffectsManager.SpawnEffect(effectType, effectPos, effectRot, Vector3.one, 1.0f, m_renderer.material);

            return false;
        }

        Detach();

        // Enable the correct directional collider
        m_hitDirection = _hitVec.normalized;
        if (m_hitDirection.x >= 0.9f)
        {
            m_posXCollider.enabled = true;
        }
        else if (m_hitDirection.x <= -0.9f)
        {
            m_negXCollider.enabled = true;
        }
        else if (m_hitDirection.z >= 0.9f)
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
        Collider[] hits = Physics.OverlapBox(checkPosition, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, m_globalSettings.m_wallLayers);
        if (hits.Length > 0)
        {
            // Hit something

            // Ignore sand
            foreach (Collider i in hits)
            {
                // Ignore self-collision
                if (i.gameObject == gameObject || (i.transform.parent && i.transform.parent.gameObject == gameObject))
                {
                    continue;
                }

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
        if (m_currentEffect == EChunkEffect.none)
        {
            MessageBus.TriggerEvent(EMessageType.chunkHitWall);
            ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.small);
            SnapChunk();
            return;
        }

        OnDeath();
    }

    // Makes the fire ball explode, hitting the four tiles around it with the fire ability
    private void FieryExplosion(Vector3 _hitDir)
    {
        Vector3 moveDir = m_prevVelocity.normalized;
        Vector3 centrePosition = transform.position - moveDir;
        SnapChunk();

        Quaternion effectRot = Quaternion.LookRotation(Vector3.down);
        Vector3 effectScale = Vector3.one * 0.5f;
        Vector3 effectPos = transform.position;

        // Creates 4 raycasts around the centre point, if the raycasts hit sand, they'll be turned to glass
        Vector3 forward = _hitDir.normalized;
        Vector3 right = new Vector3((_hitDir.x != 0.0f) ? 0.0f : 1.0f, 0.0f, (_hitDir.z != 0.0f) ? 0.0f : 1.0f);
        Vector3[] cardinalDirections = new Vector3[]
        {
            forward,
            right,
            -right,
            (forward + right).normalized,
            (forward - right).normalized,
            new Vector3(0.0f, 0.0f, 0.0f),
        };
        foreach (Vector3 direction in cardinalDirections)
        {
            Vector3 checkPosition = transform.position + direction;
            
            EffectsManager.SpawnEffect(EffectsManager.EEffectType.fieryExplosion, checkPosition, effectRot, effectScale, 0.5f);

            Collider[] colliders = Physics.OverlapBox(checkPosition, new Vector3(0.4f, 0.4f, 0.4f));

            foreach (Collider collision in colliders)
            {
                FieryExplosionCollision(collision);
            }
        }

        OnDeath();
    }

    // Called when the fiery explosion collides with something
    private void FieryExplosionCollision(Collider _collision)
    {
        SandBlock sand = _collision.GetComponent<SandBlock>();
        if (sand && !sand.m_isGlass)
        {
            sand.TurnToGlass();
        }

        Torch torch = _collision.GetComponent<Torch>();
        if (torch)
        {
            torch.AttemptToActivate();
        }

        CobraHealth cobraHealth = _collision.GetComponent<CobraHealth>();
        if (cobraHealth)
        {
            CobraHealth.Damage();
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
            //transform.position = newPos;
            transform.DOMove(newPos, 0.1f);
        }
        else
        {
            Debug.LogError("Unable to find nearest tile to snap to");
        }
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
        transform.DOMove(_tonguePos, 0.1f);
        Vector3 distance = _frogPos - transform.position;
        distance.y = 0.0f;
        float tweenTime = distance.magnitude / 3.0f;
        transform.DOMove(_frogPos, tweenTime);
        transform.DOScale(0.1f, tweenTime);
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
