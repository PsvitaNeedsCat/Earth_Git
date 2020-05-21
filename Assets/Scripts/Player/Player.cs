using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public variables
    public Animator m_animator;
    [HideInInspector] public Vector2 m_moveDirection = Vector2.zero;
    [HideInInspector] public bool m_hasKey = false;

    // Private variables
    private GlobalPlayerSettings m_settings;
    private Player m_instance;
    private PlayerController m_playerController;
    private float m_punchTimer = 0.0f; // For punch cooldown (0.0f can punch)
    private float m_raiseTimer = 0.0f; // For raise cooldown (0.0f can raise)
    [SerializeField] private TileTargeter m_tileTargeter;
    private eChunkEffect m_currentEffect = eChunkEffect.none;
    
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
        // Move player with provided input
        m_playerController.Move(m_moveDirection);

        // Update animator
        m_animator.SetBool("Running", (m_moveDirection.magnitude > 0.1f));

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
    public void TryPunch()
    {
        if (m_punchTimer <= 0.0f)
        {
            m_punchTimer = m_settings.m_punchCooldown;
            
            m_playerController.Punch(m_currentEffect);
        }
    }

    // Attempts to raise a chunk
    public void TryRaiseChunk()
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
    }

    public void ActivateTileTargeter()
    {
        m_playerController.ActivateTileTargeter();
    }

    public void DeactivateTileTargeter()
    {
        m_playerController.DeactivateTileTargeter();
    }

    // Starts the punch animation, if possible
    public void StartPunchAnim()
    {
        if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("Punch")) { return; }

        m_animator.SetTrigger("Punch");
    }

    // Starts the chunk raise animation, if possible
    public void StartRaiseChunkAnim()
    {
        if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("Summon")) { return; }

        if (m_playerController.TryConfirmChunk())
        {
            m_animator.SetTrigger("Summon");
        }
    }

    public void ChangeEffect(eChunkEffect _effect)
    {
        m_currentEffect = _effect;

        switch (_effect)
        {
            case eChunkEffect.water:
                { MessageBus.TriggerEvent(EMessageType.powerWater); break; }

            case eChunkEffect.fire:
                { MessageBus.TriggerEvent(EMessageType.powerFire); break; }

            default:
                { MessageBus.TriggerEvent(EMessageType.powerRock); break; }
        }
    }
}
