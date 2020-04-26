using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables
    public Tile m_confirmedTile = null;

    // Private variables
    private PlayerController m_instance;
    private GameObject m_tileTargeter;
    private Rigidbody m_rigidBody;
    [SerializeField] private GlobalPlayerSettings m_settings;
    [SerializeField] private GameObject m_hurtboxPrefab;
    private List<Chunk> m_activeChunks;

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

        // Set tile targeter
        TileTargeter targeter = FindObjectOfType<TileTargeter>();
        Debug.Assert(targeter, "Object of type TileTargeter.cs could not be found");
        m_tileTargeter = targeter.gameObject;

        // Set rigidbody
        m_rigidBody = GetComponent<Rigidbody>();
        Debug.Assert(m_rigidBody, "No rigidbody found on player");
    }

    // Moves the player in a given direction
    public void Move(Vector2 _direction)
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

    // Applies drag and gravity to the player
    public void ApplyDrag()
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
        Instantiate(m_hurtboxPrefab, spawnPos, transform.rotation);
    }

    public void ActivateTileTargeter()
    {
        m_tileTargeter.SetActive(true);
    }

    public void DeactivateTileTargeter()
    {
        m_tileTargeter.SetActive(false);
    }

    // Called by animator
    // Raises a given chunk
    public void RaiseChunk()
    {
        Debug.Assert(m_confirmedTile, "Confirmed tile was null");

        Chunk newChunk = m_confirmedTile.TryRaiseChunk();

        if (newChunk)
        {
            // Add chunk to list
            if (m_activeChunks.Count >= m_settings.m_maxChunks)
            {
                RemoveChunk(m_activeChunks[0]);
            }

            // Create new chunk
            m_activeChunks.Add(newChunk);
        }

        m_confirmedTile = null;
    }

    // Removes a given chunk from the world
    public void RemoveChunk(Chunk _chunk)
    {
        for (int i = 0; i < m_activeChunks.Count; i++)
        {
            if (m_activeChunks[i].GetInstanceID() == _chunk.GetInstanceID())
            {
                Destroy(m_activeChunks[i].gameObject);
                m_activeChunks.RemoveAt(i);
                return;
            }
        }

        Debug.LogError("Couldn't find chunk to be removed");
    }
}
