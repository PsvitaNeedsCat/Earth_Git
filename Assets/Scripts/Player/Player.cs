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
    public static Dictionary<EChunkEffect, bool> s_activePowers = new Dictionary<EChunkEffect, bool>()
    {
        { EChunkEffect.none, true },
        { EChunkEffect.water, false },
        { EChunkEffect.fire, false }
    };
    public static EChunkEffect s_currentEffect = EChunkEffect.none;

    // Private variables
    private GlobalPlayerSettings m_settings;
    private Player m_instance;
    private PlayerController m_playerController;
    [SerializeField] private TileTargeter m_tileTargeter = null;
    private CrystalSelection m_crystalUI;
    private Vector3 m_rStickDir = Vector3.zero;
    [SerializeField] private ParticleSystem[] m_powerParticles = new ParticleSystem[] { };

    // Max speed that the player will reach with their current drag (it's not capped to this, this was found via testing) (used for animation blend tree)
    private readonly float m_maxSpeed = 1.6f;

    // Unlocks a power for use
    public void PowerUnlocked(EChunkEffect _power)
    {
        s_activePowers[_power] = true;
        UpdateUI();
    }
    public void ResetPowers()
    {
        // Reset all the powers except the first (rock)
        for(int i = 1; i < s_activePowers.Count; i++)
        {
            s_activePowers[(EChunkEffect)i] = false;
        }

        UpdateUI();
    }
    public EChunkEffect GetCurrentPower()
    {
        return s_currentEffect;
    }

    public void Pause()
    {
        m_playerController.Pause();
    }
    public void UnPause()
    {
        m_playerController.UnPause();
    }
    public void ContinueDialogue()
    {
        m_playerController.ContinueDialogue();
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
            if (m_rStickDir != Vector3.zero && m_moveDirection == Vector2.zero)
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
        m_playerController.Punch(s_currentEffect);
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

    // If the given power is unlocked, it selects it
    public void TryChangeEffect(EChunkEffect _effect)
    {
        // Do not let the player change if the power is not unlocked
        if (!s_activePowers[_effect]) { return; }

        // Change the player's power
        s_currentEffect = _effect;

        // Update display sprite
        UpdateUI();

        // Particles
        m_powerParticles[(int)_effect].Play();

        switch (_effect)
        {
            case EChunkEffect.water:
                {
                    MessageBus.TriggerEvent(EMessageType.powerWater);
                    break;
                }

            case EChunkEffect.fire:
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

    // Checks if the power in the given d-pad direction is unlocked and selects it
    public void TryChangeEffect(Vector2 _dpadDir)
    {
        // Down
        if (_dpadDir == Vector2.down)
        {
            return;
        }

        // Turn vector2 into EChunkEffect
        EChunkEffect effect = EChunkEffect.none;

        // Left/Right
        if (_dpadDir == Vector2.left)
        {
            effect = EChunkEffect.water;
        }
        else if (_dpadDir == Vector2.right)
        {
            effect = EChunkEffect.fire;
        }

        TryChangeEffect(effect);
    }

    // Called by PlayerInput - rotates the power CW or CCW
    public void RotateCurrentPower(float _dir)
    {
        EChunkEffect newPower;

        // If at the top of the enum
        if (_dir < 0.0f && s_currentEffect == EChunkEffect.none)
        {
            // Go to the end
            newPower = EChunkEffect.fire;
        }
        // If at the bottom of the enum
        else if (_dir > 0.0f && s_currentEffect == EChunkEffect.fire)
        {
            newPower = EChunkEffect.none;
        }
        // Otherwise, increment by direction
        else
        {
            newPower = (EChunkEffect)((int)s_currentEffect + _dir);
        }

        // Loop around the wheel
        if (!s_activePowers[newPower])
        {
            s_currentEffect = newPower;
            RotateCurrentPower(_dir);
        }

        TryChangeEffect(newPower);
    }

    private void UpdateUI()
    {
        int num = -1;
        for (int i = 0; i < s_activePowers.Count; i++)
        {
            if (s_activePowers[(EChunkEffect)i])
            {
                ++num;
            }
        }

        bool[] active = new bool[3];
        for (int i = 0; i < s_activePowers.Count; i++) { active[i] = s_activePowers[(EChunkEffect)i]; }
        m_crystalUI.UpdateUnlocked(active);
        m_crystalUI.UpdateSelected((int)s_currentEffect);
    }

    // Will try to interact with whatever is closest
    public void TryInteract()
    {
        // Check for the closest interactable
        Interactable.s_closest = null;
        MessageBus.TriggerEvent(EMessageType.interact);

        // There are no interactables
        if (!Interactable.s_closest) { return; }

        // Closest interactable is outside the maximum distance
        if (Interactable.s_closest.m_distToPlayer > m_settings.m_maxInteractableDist) { return; }

        // If everything is good, invoke
        m_playerController.Interact();
    }

    // Debug - remove on build
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            s_activePowers[EChunkEffect.water] = !s_activePowers[EChunkEffect.water];
            if (s_activePowers[EChunkEffect.water]) { TryChangeEffect(EChunkEffect.water); }
            else { TryChangeEffect(EChunkEffect.none); }
            Debug.Log("Water power: " + s_activePowers[EChunkEffect.water]);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            s_activePowers[EChunkEffect.fire] = !s_activePowers[EChunkEffect.fire];
            if (s_activePowers[EChunkEffect.fire]) { TryChangeEffect(EChunkEffect.fire); }
            else { TryChangeEffect(EChunkEffect.none); }
            Debug.Log("Fire power: " + s_activePowers[EChunkEffect.fire]);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Player got key");
            m_hasKey = true;
        }
    }
}
