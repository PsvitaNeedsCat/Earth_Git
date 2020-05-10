using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables
    public Tile m_confirmedTile = null;

    // Serialized Variables
    [SerializeField] private GameObject m_hurtboxPrefab;
    [SerializeField] private TileTargeter m_tileTargeter;

    // Private variables
    private PlayerController m_instance;
    private Rigidbody m_rigidBody;
    private GlobalPlayerSettings m_settings;
    private HealthComponent m_health;

    private void Awake()
    {
        // Only one instance
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("A second instance of PlayerController.cs was instantiated");
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }

        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");

        // Set health
        m_health = GetComponent<HealthComponent>();
        m_health.Init(m_settings.m_defaultMaxHealth, m_settings.m_defaultMaxHealth, OnHurt, null, OnDeath);

        // Set rigidbody
        m_rigidBody = GetComponent<Rigidbody>();
        Debug.Assert(m_rigidBody, "No rigidbody found on player");
    }

    private void OnHurt()
    {
        MessageBus.TriggerEvent(EMessageType.playerHurt);
    }

    private void OnDeath()
    {

    }

    // Moves the player in a given direction
    public void Move(Vector2 _direction)
    {
        // Only rotate and move character if there is directional input
        if (_direction.magnitude > 0.1f)
        {
            // Get yaw
            float yaw = Camera.main.transform.rotation.eulerAngles.y;

            // Convert to 3D
            Vector3 moveDir = new Vector3(_direction.x, 0.0f, _direction.y);

            // Rotate direction vector by yaw
            moveDir = Quaternion.Euler(new Vector3(0.0f, yaw, 0.0f)) * moveDir;

            // Set look direction
            transform.forward = moveDir;

            // Add force
            m_rigidBody.AddForce(moveDir.normalized * m_settings.m_moveForce, ForceMode.Impulse);
        }

        ApplyDrag();
    }

    // Applies drag and gravity to the player
    private void ApplyDrag()
    {
        // Drag
        Vector3 vel = m_rigidBody.velocity;
        vel.x *= (0.98f / m_settings.m_drag);
        vel.z *= (0.98f / m_settings.m_drag);
        m_rigidBody.velocity = vel;

        // Gravity
        m_rigidBody.AddForce(Vector3.up * m_settings.m_gravity);
    }

    // Makes the player punch
    public void Punch()
    {
        Vector3 spawnPos = transform.position;

        spawnPos += (transform.forward.normalized * m_settings.m_hurtboxMoveBy);

        // 
        spawnPos.y += GetComponent<Collider>().bounds.size.y * 0.5f;

        // Spawn in
        Hurtbox hurtbox = Instantiate(m_hurtboxPrefab, spawnPos, transform.rotation).GetComponent<Hurtbox>();
        hurtbox.SetPlayerPos(transform.position);
    }

    public void ActivateTileTargeter()
    {
        m_tileTargeter.gameObject.SetActive(true);
    }

    public void DeactivateTileTargeter()
    {
        m_tileTargeter.gameObject.SetActive(false);
    }

    public bool TryConfirmChunk()
    {
        Tile tile = m_tileTargeter.GetClosest();

        if (tile)
        {
            bool tileFree = !tile.IsOccupied();

            if (tileFree) { m_confirmedTile = tile; }
            else { m_confirmedTile = null; }

            return tileFree;
        }
        else
        {
            return false;
        }
    }

    // Raises a given chunk
    public void RaiseChunk()
    {
        Debug.Assert(m_confirmedTile, "Confirmed tile was null");

        Chunk newChunk = m_confirmedTile.TryRaiseChunk();

        if (newChunk)
        {
            if (ChunkManager.NumChunks() > m_settings.m_maxChunks)
            {
                ChunkManager.RemoveOldest();
            }
        }

        m_confirmedTile = null;
    }
}
