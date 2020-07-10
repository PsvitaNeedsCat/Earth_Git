using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Public variables
    public Animator m_animator;
    [HideInInspector] public Vector2 m_moveDirection = Vector2.zero;
    [HideInInspector] public bool m_hasKey = false;
    public static Dictionary<eChunkEffect, bool> m_activePowers = new Dictionary<eChunkEffect, bool>()
    {
        { eChunkEffect.none, true },
        { eChunkEffect.water, false },
        { eChunkEffect.fire, false }
    };

    // Private variables
    private GlobalPlayerSettings m_settings;
    private Player m_instance;
    private PlayerController m_playerController;
    [SerializeField] private TileTargeter m_tileTargeter;
    private eChunkEffect m_currentEffect = eChunkEffect.none;
    private CrystalSelection m_crystalUI;

    // Max speed that the player will reach with their current drag
    private readonly float m_maxSpeed = 1.6f;

    // Unlocks a power for use
    public void PowerUnlocked(eChunkEffect _power)
    {
        m_activePowers[_power] = true;
        UpdateUI();
    }
    public void ResetPowers()
    {
        // Reset all the powers except the first (rock)
        for(int i = 1; i < m_activePowers.Count; i++)
        {
            m_activePowers[(eChunkEffect)i] = false;
        }

        UpdateUI();
    }

    public void Pause() => m_playerController.Pause();
    public void UnPause() => m_playerController.UnPause();
    public void ContinueDialogue() => m_playerController.ContinueDialogue();
    public void ResetTileTargeter() => m_tileTargeter.ResetDirection();

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

        m_crystalUI = FindObjectOfType<CrystalSelection>();
        UpdateUI();
    }

    private void FixedUpdate()
    {
        // Move player with provided input
        m_playerController.Move(m_moveDirection);

        // Update the animator's 'MoveInput' boolean based on player input
        m_animator.SetBool("MoveInput", m_moveDirection.magnitude > 0.01f);

        // Set blend tree value with player's speed
        Vector3 playerVelocity = m_playerController.m_rigidBody.velocity;
        playerVelocity.y = 0.0f;
        float playerSpeed = playerVelocity.magnitude;
        m_animator.SetFloat("Blend", Mathf.Clamp01(playerSpeed / m_maxSpeed));
    }

    // Depending on whether the player is trying to raise a chunk or not,
    // It will either move the player or move the tile targeter
    public void SetLAnalogDirection(Vector2 _dir, bool _isTargeting)
    {
        // Player is trying to target a tile
        if (_isTargeting)
        {
            // Set the tile targeter direction
            m_tileTargeter.SetTargetDirection(_dir, transform.position);

            // Reset move direction
            m_moveDirection = Vector2.zero;

            return;
        }

        // Player is trying to move
        m_moveDirection = _dir;
    }

    // Attempts to punch
    public void TryPunch()
    {
        m_playerController.Punch(m_currentEffect);
    }

    // Attempts to raise a chunk
    public void TryRaiseChunk()
    {
        // Try confirm chunk
        Tile closestTile = m_tileTargeter.GetClosest();

        // Closest tile exists
        // And is free
        if (closestTile && !closestTile.IsOccupied())
        {
            // CHUNK IS GOOD TO RAISE

            m_playerController.m_confirmedTile = closestTile;

            // Remove this when animator is set
            m_playerController.RaiseChunk();

            // Set animation trigger
        }
    }

    public void ActivateTileTargeter()
    {
        SetLAnalogDirection(m_moveDirection, true);
        m_moveDirection = Vector2.zero;
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

    public void TryChangeEffect(eChunkEffect _effect)
    {
        // Do not let the player change if the power is not unlocked
        if (!m_activePowers[_effect]) { return; }

        // Change the player's power
        m_currentEffect = _effect;

        // Update display sprite
        UpdateUI();

        switch (_effect)
        {
            case eChunkEffect.water:
                {
                    MessageBus.TriggerEvent(EMessageType.powerWater);
                    break;
                }

            case eChunkEffect.fire:
                {
                    MessageBus.TriggerEvent(EMessageType.powerFire);
                    break;
                }

            default:
                {
                    MessageBus.TriggerEvent(EMessageType.powerRock);
                    break;
                }
        }
    }

    private void UpdateUI()
    {
        int num = -1;
        for (int i = 0; i < m_activePowers.Count; i++)
        {
            if (m_activePowers[(eChunkEffect)i])
            {
                ++num;
            }
        }

        bool[] active = new bool[3];
        for (int i = 0; i < m_activePowers.Count; i++) { active[i] = m_activePowers[(eChunkEffect)i]; }
        m_crystalUI.UpdateUnlocked(active);
        m_crystalUI.UpdateSelected((int)m_currentEffect);
    }

    // Will try to interact with whatever is closest
    public void TryInteract()
    {
        // Check for the closest interactable
        Interactable.m_closest = null;
        MessageBus.TriggerEvent(EMessageType.interact);

        // There are no interactables
        if (!Interactable.m_closest) { return; }

        // Closest interactable is outside the maximum distance
        if (Interactable.m_closest.m_distToPlayer > m_settings.m_maxInteractableDist) { return; }

        // If everything is good, invoke
        m_playerController.Interact();
    }

    // Debug - remove on build
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            m_activePowers[eChunkEffect.water] = !m_activePowers[eChunkEffect.water];
            if (m_activePowers[eChunkEffect.water]) { TryChangeEffect(eChunkEffect.water); }
            else { TryChangeEffect(eChunkEffect.none); }
            Debug.Log("Water power: " + m_activePowers[eChunkEffect.water]);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            m_activePowers[eChunkEffect.fire] = !m_activePowers[eChunkEffect.fire];
            if (m_activePowers[eChunkEffect.fire]) { TryChangeEffect(eChunkEffect.fire); }
            else { TryChangeEffect(eChunkEffect.none); }
            Debug.Log("Fire power: " + m_activePowers[eChunkEffect.fire]);
        }
    }
}
