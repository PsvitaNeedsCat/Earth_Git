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
    [SerializeField] private TileTargeter m_tileTargeter = null;
    private eChunkEffect m_currentEffect = eChunkEffect.none;
    private CrystalSelection m_crystalUI;
    private Vector3 m_rStickDir = Vector3.zero;

    // Max speed that the player will reach with their current drag (it's not capped to this, this was found via testing) (used for animation blend tree)
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
    public eChunkEffect GetCurrentPower()
    {
        return m_currentEffect;
    }

    public void Pause() => m_playerController.Pause();
    public void UnPause() => m_playerController.UnPause();
    public void ContinueDialogue() => m_playerController.ContinueDialogue();

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

    // Modifies movement - called by PlayerInput
    public void SetLAnalogDirection(Vector2 _dir)
    {
        // Player is trying to move
        m_moveDirection = _dir;

        m_tileTargeter.UpdateDirection(transform.position);
    }

    // Modifies targeting - called by PlayerInput
    public void SetRAnalogDirection(Vector2 _dir)
    {
        m_rStickDir = Camera.main.RelativeDirection2(_dir);

        m_tileTargeter.SetTargetDirection(_dir, transform.position);

        m_tileTargeter.gameObject.SetActive(_dir != Vector2.zero);
    }

    // Called by PlayerInput
    public void BeginPunchAnimation()
    {
        if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
        {
            if (m_rStickDir != Vector3.zero)
            {
                transform.forward = m_rStickDir;
            }

            m_animator.SetTrigger("Punch");
        }
    }

    // Called by PlayerInput
    public void BeginRaiseAnimation()
    {
        if (!m_tileTargeter.gameObject.activeSelf)
        {
            return;
        }

        if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("Summon") && m_playerController.TryConfirmChunk())
        {
            m_animator.SetTrigger("Summon");
        }
    }

    // Called by punch animation
    public void TryPunch()
    {
        m_playerController.Punch(m_currentEffect);
    }

    // Called by raise animation
    public void TryRaiseChunk()
    {
        // Closest tile exists
        // And is free
        if (m_playerController.m_confirmedTile && !m_playerController.m_confirmedTile.IsOccupied())
        {
            // CHUNK IS GOOD TO RAISE

            // Remove this when animator is set
            m_playerController.RaiseChunk();
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
