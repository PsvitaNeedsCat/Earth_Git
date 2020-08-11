using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Private variables
    public static InputMaster s_controls;
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
        s_controls = new InputMaster();
        m_player = GetComponent<Player>();
        Debug.Assert(m_player, "No Player.cs found on player object");

        // Controls //

        // Movement
        s_controls.PlayerMovement.Movement.performed += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>());
        s_controls.PlayerMovement.Movement.canceled += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>());
        s_controls.PlayerMovement.KeyboardMovement.performed += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>());
        s_controls.PlayerMovement.KeyboardMovement.canceled += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>());
        // Interact
        s_controls.PlayerMovement.Interact.performed += _ => m_player.TryInteract();
        // Punch
        s_controls.PlayerCombat.Punch.performed += _ => m_player.BeginPunchAnimation();
        // Raise Chunk
        s_controls.PlayerCombat.Raise.performed += _ => m_player.BeginRaiseAnimation();
        // Target
        s_controls.PlayerCombat.Target.performed += ctx => m_player.SetRAnalogDirection(ctx.ReadValue<Vector2>());
        s_controls.PlayerCombat.Target.canceled += ctx => m_player.SetRAnalogDirection(ctx.ReadValue<Vector2>());
        s_controls.PlayerCombat.KeyboardTarget.performed += ctx => m_player.SetRAnalogDirection(ctx.ReadValue<Vector2>());
        s_controls.PlayerCombat.KeyboardTarget.canceled += ctx => m_player.SetRAnalogDirection(ctx.ReadValue<Vector2>());
        // Change powers
        s_controls.PlayerCombat.PowerSelection.performed += ctx => m_player.TryChangeEffect(ctx.ReadValue<Vector2>());
        s_controls.PlayerCombat.PowerRotation.performed += ctx => m_player.RotateCurrentPower(ctx.ReadValue<float>());
        // Pause
        s_controls.PlayerMovement.Pause.performed += _ => m_player.Pause();
        s_controls.Pause.UnPause.performed += _ => m_player.UnPause();
        // Dialogue
        s_controls.Dialogue.Continue.performed += _ => m_player.ContinueDialogue();

        // Set init values
        SetMovement(m_defaultMovement);
        SetCombat(m_defaultCombat);
        s_controls.Pause.Disable();
        s_controls.Dialogue.Disable();
    }

    private void OnDestroy()
    {
        if (s_instance == this) { s_controls.Disable(); }
    }

    public void SetMovement(bool _active)
    {
        if (_active) { s_controls.PlayerMovement.Enable(); }
        else { s_controls.PlayerMovement.Disable(); }
    }
    
    public bool HasMovement()
    {
        return s_controls.PlayerMovement.enabled;
    }

    public void SetCombat(bool _active)
    {
        if (_active) { s_controls.PlayerCombat.Enable(); }
        else { s_controls.PlayerCombat.Disable(); }
    }

    public void SetPause(bool _active)
    {
        if (_active)
        {
            m_prevMovement = s_controls.PlayerMovement.enabled;
            m_prevCombat = s_controls.PlayerCombat.enabled;
            SetMovement(false);
            SetCombat(false);
            s_controls.Pause.Enable();
        }
        else
        {
            s_controls.Pause.Disable();
            SetMovement(m_prevMovement);
            SetCombat(m_prevCombat);
        }
    }

    public void SetDialogue(bool _active)
    {
        if (_active)
        {
            m_prevCombat = s_controls.PlayerCombat.enabled;
            m_prevMovement = s_controls.PlayerMovement.enabled;
            SetMovement(false);
            SetCombat(false);
            s_controls.Pause.Disable();
            s_controls.Dialogue.Enable();
        }
        else
        {
            SetMovement(m_prevMovement);
            SetCombat(m_prevCombat);
            s_controls.Dialogue.Disable();
        }
    }
}
