using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public enum eChunkEffect
{
    none,
    waterTrail
}

[RequireComponent(typeof(Rigidbody))]
public class Chunk : MonoBehaviour
{
    // Public variables
    [HideInInspector] public eChunkEffect m_currentEffect = eChunkEffect.none;
    [HideInInspector] public eChunkType m_chunkType = eChunkType.none;
    [HideInInspector] public bool m_isRaised = false;

    // Serialized variables
    [SerializeField] private ChunkSettings m_settings;

    // Private variables
    private Rigidbody m_rigidBody;
    private Vector3 m_spawnPos;
    GlobalChunkSettings m_globalSettings;
    private HealthComponent m_healthComp;

    // Hitboxes
    [SerializeField] private Collider m_posXCollider;
    [SerializeField] private Collider m_negXCollider;
    [SerializeField] private Collider m_posZCollider;
    [SerializeField] private Collider m_negZCollider;
    [SerializeField] private Collider m_mainCollider;

    // Chunks automatically added and removed to chunk manager over lifetime
    [ExecuteAlways] private void OnEnable() => ChunkManager.AddChunk(this);
    [ExecuteAlways] private void OnDisable() => ChunkManager.RemoveChunk(this);

    private void Awake()
    {
        // Set values
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.isKinematic = true;

        // Setup health component
        m_healthComp = GetComponent<HealthComponent>();
        m_healthComp.Init(m_settings.m_maxHealth, m_settings.m_maxHealth);
        m_healthComp.OnHurt = this.OnHurt;
        m_healthComp.OnDeath = this.OnDeath;

        m_spawnPos = transform.position;

        m_globalSettings = Resources.Load<GlobalChunkSettings>("ScriptableObjects/GlobalChunkSettings");
    }

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
        // If colliding with hurtbox, ignore
        Hurtbox hurtboxCheck = other.GetComponent<Hurtbox>();
        if (hurtboxCheck) { return; }

        // Did not hit ground or player
        if (other.tag != "Ground" && other.tag != "Player")
        {
            if (IsAgainstWall(m_rigidBody.velocity.normalized))
            {
                SnapChunk();
            }
        }
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
        MessageBus.TriggerEvent(EMessageType.chunkDestroyed);
        Destroy(this.gameObject);
    }

    // Pushes the chunk in a specific direction when hit by the player
    public void Hit(Vector3 _hitVec)
    {
        if (!m_isRaised) { return; }

        if (IsAgainstWall(_hitVec))
        {
            // Play sound
            m_healthComp.Health -= 1;
            return;
        }

        Detach();

        // Enable the correct directional collider
        Vector3 cardinal = _hitVec.normalized;
        if (cardinal.x >= 0.9f) { m_posXCollider.enabled = true; }
        else if (cardinal.x <= -0.9f) { m_negXCollider.enabled = true; }
        else if (cardinal.z >= 0.9f) { m_posZCollider.enabled = true; }
        else { m_negZCollider.enabled = true; } // else if (cardinal.z <= -1.0f)

        m_rigidBody.AddForce(_hitVec, ForceMode.Impulse);

        // Play sound
    }

    // Checks if the chunk is against a wall using a raycast
    private bool IsAgainstWall(Vector3 _hitVec)
    {
        // Start position - almost the bottom of the chunk
        Vector3 checkPosition = transform.position;
        checkPosition.y -= m_globalSettings.m_chunkHeight * 0.5f - 0.5f;

        // Raycast in the direction of the hit vector for half a chunk's length
        RaycastHit hit;
        if (Physics.Raycast(checkPosition, _hitVec, out hit, m_globalSettings.m_wallCheckDistance, m_globalSettings.m_wallLayers))
        {
            // Hit something
            return true;
        }

        return false;
    }

    // Makes the chunk moveable
    public void Detach()
    {
        m_rigidBody.isKinematic = false;
        m_mainCollider.enabled = false;
        m_rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // Snaps a chunk to the nearest grid tile
    private void SnapChunk()
    {
        // Play sound

        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.isKinematic = true;
        m_rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Change colliders
        m_posXCollider.enabled = false;
        m_negXCollider.enabled = false;
        m_posZCollider.enabled = false;
        m_negZCollider.enabled = false;
        m_mainCollider.enabled = true;

        // Find nearest grid tile
        Tile nearest = Grid.FindClosestTile(transform.position, true);

        // Snap to the nearest grid tile
        if (nearest)
        {
            Vector3 newPos = nearest.transform.position;
            newPos.y = transform.position.y;
            transform.position = newPos;
        }
        else { Debug.LogError("Unable to find nearest tile to snap to"); }
    }
}
