using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Private variables
    private InputMaster m_controls;
    private Player m_player;

    private bool m_prevMovement;
    private bool m_prevCombat;

    // Initial values
    [SerializeField] private bool m_defaultMovement = true;
    [SerializeField] private bool m_defaultCombat = true;

    // Only one instance
    private static PlayerInput s_instance;

    private void Awake()
    {
        // Only one instance of the player
        if (s_instance != null && s_instance != this)
        {
            Debug.LogError("A second instance of 'PlayerInput.cs' wasinstantiated");
            Destroy(this.gameObject);
        }
        else
        {
            s_instance = this;
        }

        // Init
        m_controls = new InputMaster();
        m_player = GetComponent<Player>();
        Debug.Assert(m_player, "No Player.cs found on player object");

        // Controls //

        // Movement
        m_controls.PlayerMovement.Movement.performed += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>());
        m_controls.PlayerMovement.Movement.canceled += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>());
        m_controls.PlayerMovement.KeyboardMovement.performed += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>());
        m_controls.PlayerMovement.KeyboardMovement.canceled += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>());
        // Interact
        m_controls.PlayerMovement.Interact.performed += _ => m_player.TryInteract();
        // Punch
        m_controls.PlayerCombat.Punch.performed += _ => m_player.BeginPunchAnimation();
        // Raise Chunk
        m_controls.PlayerCombat.Raise.performed += _ => m_player.BeginRaiseAnimation();
        // Target
        m_controls.PlayerCombat.Target.performed += ctx => m_player.SetRAnalogDirection(ctx.ReadValue<Vector2>());
        m_controls.PlayerCombat.Target.canceled += ctx => m_player.SetRAnalogDirection(ctx.ReadValue<Vector2>());
        m_controls.PlayerCombat.KeyboardTarget.performed += ctx => m_player.SetRAnalogDirection(ctx.ReadValue<Vector2>());
        m_controls.PlayerCombat.KeyboardTarget.canceled += ctx => m_player.SetRAnalogDirection(ctx.ReadValue<Vector2>());
        // Change powers
        m_controls.PlayerCombat.NoPower.performed += _ => m_player.TryChangeEffect(EChunkEffect.none);
        m_controls.PlayerCombat.WaterPower.performed += _ => m_player.TryChangeEffect(EChunkEffect.water);
        m_controls.PlayerCombat.FirePower.performed += _ => m_player.TryChangeEffect(EChunkEffect.fire);
        // Pause
        m_controls.PlayerMovement.Pause.performed += _ => m_player.Pause();
        m_controls.Pause.UnPause.performed += _ => m_player.UnPause();
        // Dialogue
        m_controls.Dialogue.Continue.performed += _ => m_player.ContinueDialogue();

        // Set init values
        SetMovement(m_defaultMovement);
        SetCombat(m_defaultCombat);
        m_controls.Pause.Disable();
        m_controls.Dialogue.Disable();
    }

    private void OnDestroy()
    {
        if (s_instance == this) { m_controls.Disable(); }
    }

    public void SetMovement(bool _active)
    {
        if (_active) { m_controls.PlayerMovement.Enable(); }
        else { m_controls.PlayerMovement.Disable(); }
    }
    
    public bool HasMovement()
    {
        return m_controls.PlayerMovement.enabled;
    }

    public void SetCombat(bool _active)
    {
        if (_active) { m_controls.PlayerCombat.Enable(); }
        else { m_controls.PlayerCombat.Disable(); }
    }

    public void SetPause(bool _active)
    {
        if (_active)
        {
            m_prevMovement = m_controls.PlayerMovement.enabled;
            m_prevCombat = m_controls.PlayerCombat.enabled;
            SetMovement(false);
            SetCombat(false);
            m_controls.Pause.Enable();
        }
        else
        {
            m_controls.Pause.Disable();
            SetMovement(m_prevMovement);
            SetCombat(m_prevCombat);
        }
    }

    public void SetDialogue(bool _active)
    {
        if (_active)
        {
            m_prevCombat = m_controls.PlayerCombat.enabled;
            m_prevMovement = m_controls.PlayerMovement.enabled;
            SetMovement(false);
            SetCombat(false);
            m_controls.Pause.Disable();
            m_controls.Dialogue.Enable();
        }
        else
        {
            SetMovement(m_prevMovement);
            SetCombat(m_prevCombat);
            m_controls.Dialogue.Disable();
        }
    }
}
