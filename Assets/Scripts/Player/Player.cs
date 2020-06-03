using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

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
    private static Dictionary<eChunkEffect, bool> m_activePowers = new Dictionary<eChunkEffect, bool>()
    {
        { eChunkEffect.none, true },
        { eChunkEffect.water, false },
        { eChunkEffect.fire, false }
    };

    // Unlocks a power for use
    public void PowerUnlocked(eChunkEffect _power) => m_activePowers[_power] = true;
    public void ResetPowers()
    {
        // Reset all the powers except the first (rock)
        for(int i = 1; i < m_activePowers.Count; i++)
        {
            m_activePowers[(eChunkEffect)i] = false;
        }
    }

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

    public void TryChangeEffect(eChunkEffect _effect)
    {
        // Do not let the player change if the power is not unlocked
        if (!m_activePowers[_effect]) { return; }

        // Change the player's power
        m_currentEffect = _effect;

        switch (_effect)
        {
            case eChunkEffect.water:
                {
                    MessageBus.TriggerEvent(EMessageType.powerWater);
                    GameObject.Find("TempTxt").GetComponent<TextMeshProUGUI>().text = "Water";
                    break;
                }

            case eChunkEffect.fire:
                {
                    MessageBus.TriggerEvent(EMessageType.powerFire);
                    GameObject.Find("TempTxt").GetComponent<TextMeshProUGUI>().text = "Fire";
                    break;
                }

            default:
                {
                    MessageBus.TriggerEvent(EMessageType.powerRock);
                    GameObject.Find("TempTxt").GetComponent<TextMeshProUGUI>().text = "Rock";
                    break;
                }
        }
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
        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    m_activePowers[eChunkEffect.water] = !m_activePowers[eChunkEffect.water];
        //    if (m_activePowers[eChunkEffect.water]) { TryChangeEffect(eChunkEffect.water); }
        //    else { TryChangeEffect(eChunkEffect.none); }
        //    Debug.Log("Water power: " + m_activePowers[eChunkEffect.fire]);
        //}
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    m_activePowers[eChunkEffect.fire] = !m_activePowers[eChunkEffect.fire];
        //    if (m_activePowers[eChunkEffect.fire]) { TryChangeEffect(eChunkEffect.fire); }
        //    else { TryChangeEffect(eChunkEffect.none); }
        //    Debug.Log("Fire power: " + m_activePowers[eChunkEffect.fire]);
        //}
    }
}
