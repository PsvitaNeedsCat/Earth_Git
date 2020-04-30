using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public variables
    [HideInInspector] public Vector2 m_moveDirection = Vector2.zero;

    // Private variables
    private GlobalPlayerSettings m_settings;
    private Player m_instance;
    private PlayerController m_playerController;
    private float m_punchTimer = 0.0f; // For punch cooldown (0.0f can punch)
    private float m_raiseTimer = 0.0f; // For raise cooldown (0.0f can raise)
    [SerializeField] private TileTargeter m_tileTargeter;

    private void Awake()
    {
        // Only ever one instance of this
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("A second instance of Player.cs was instantiated");
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }

        m_playerController = GetComponent<PlayerController>();
        Debug.Assert(m_playerController, "PlayerController.cs is not a component of player object");

        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
    }

    private void FixedUpdate()
    {
        // Move player
        if (m_moveDirection != Vector2.zero)
        //if (Mathf.Abs(m_moveDirection.x) > 0.5f || Mathf.Abs(m_moveDirection.y) > 0.5f)
        {
            // Set animation
            m_playerController.Move(m_moveDirection);
        }
        else
        {
            // Set animation
        }

        m_playerController.ApplyDrag();

        // Punch cooldown
        if (m_punchTimer > 0.0f)
        {
            m_punchTimer -= Time.fixedDeltaTime;
        }

        // Raise cooldown
        if (m_raiseTimer > 0.0f)
        {
            m_raiseTimer -= Time.fixedDeltaTime;
        }
    }

    // Attempts to punch
    public void AttemptPunch()
    {
        if (m_punchTimer <= 0.0f)
        {
            m_punchTimer = m_settings.m_punchCooldown;
            
            m_playerController.Punch();
        }
    }

    // Attempts to raise a chunk
    public void AttemptRaiseChunk()
    {
        if (m_raiseTimer <= 0.0f)
        {
            // Try confirm chunk
            Tile closestTile = m_tileTargeter.GetClosest();

            // Closest tile exists
            // And is free
            if (closestTile && !closestTile.IsOccupied())
            {
                // CHUNK IS GOOD TO RAISE

                m_raiseTimer = m_settings.m_raiseCooldown;

                m_playerController.m_confirmedTile = closestTile;

                // Remove this when animator is set
                m_playerController.RaiseChunk();

                // Set animation trigger
            }
        }

        DeactivateTileTargeter();
    }

    public void ActivateTileTargeter()
    {
        m_playerController.ActivateTileTargeter();
    }

    private void DeactivateTileTargeter()
    {
        m_playerController.DeactivateTileTargeter();
    }
}
