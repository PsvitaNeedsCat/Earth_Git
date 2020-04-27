using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public variables
    public Vector2 m_moveDirection = Vector2.zero;

    // Private variables
    [SerializeField] private GlobalPlayerSettings m_settings;
    private Player m_instance;
    private PlayerController m_playerController;
    private float m_punchTimer = 0.0f; // For punch cooldown (0.0f can punch)
    private float m_raiseTimer = 0.0f; // For raise cooldown (0.0f can raise)
    private TileTargeter m_tileTargeter;

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

        m_tileTargeter = FindObjectOfType<TileTargeter>();
        Debug.Assert(m_tileTargeter, "Object of type TileTargeter.cs could not be found");
    }

    private void FixedUpdate()
    {
        // Move player
        if (m_moveDirection != Vector2.zero)
        {
            // Set animation

            m_playerController.Move(m_moveDirection);
        }
        else
        {
            // Set animation
        }

        // Punch cooldown
        if (m_punchTimer > 0.0f)
        {
            m_punchTimer -= Time.fixedDeltaTime;
        }
    }

    // Attempts to punch
    public void AttemptPunch()
    {
        if (m_punchTimer <= 0.0f)
        {
            m_punchTimer = m_settings.m_punchCooldown;

            // Punch
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
