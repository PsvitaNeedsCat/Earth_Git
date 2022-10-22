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
    private CheatConsole m_cheatConsole;

    private bool m_prevMovement;
    private bool m_prevCombat;

    // Initial values
    [SerializeField] private bool m_defaultMovement = true;
    [SerializeField] private bool m_defaultCombat = true;
    [SerializeField] private bool m_allowPowerSelection = true;

    private bool m_canJump = false;

    // Only one instance
    private static PlayerInput s_instance;

    private void Awake()
    {
        // Only one instance of the player
        if (s_instance != null && s_instance != this)
        {
            Debug.LogError("A second instance of 'PlayerInput.cs' wasinstantiated");
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
        }

        // Init
        s_controls = new InputMaster();
        m_player = GetComponent<Player>();
        m_cheatConsole = GetComponent<CheatConsole>();
        Debug.Assert(m_player, "No Player.cs found on player object");

        // Controls //

        // Movement
        s_controls.PlayerMovement.Movement.performed += ctx => TrySetAnalogDirection(true, ctx.ReadValue<Vector2>());
        s_controls.PlayerMovement.Movement.canceled += ctx => TrySetAnalogDirection(true, ctx.ReadValue<Vector2>());
        s_controls.PlayerMovement.KeyboardMovement.performed += ctx => TrySetAnalogDirection(true, ctx.ReadValue<Vector2>());
        s_controls.PlayerMovement.KeyboardMovement.canceled += ctx => TrySetAnalogDirection(true, ctx.ReadValue<Vector2>());

        s_controls.PlayerMovement.Jump.performed += _ => m_player.Jump();

        // Interact
        s_controls.PlayerMovement.Interact.performed += _ => m_player.TryInteract();
        // Punch
        s_controls.PlayerCombat.Punch.performed += _ => m_player.BeginPunchAnimation();
        // Raise Chunk
        s_controls.PlayerCombat.Raise.performed += _ => m_player.BeginRaiseAnimation();
        // Target
        //s_controls.PlayerCombat.Target.performed += ctx => TrySetAnalogDirection(false, ctx.ReadValue<Vector2>());
        //s_controls.PlayerCombat.Target.canceled += ctx => TrySetAnalogDirection(false, ctx.ReadValue<Vector2>());
        s_controls.PlayerCombat.KeyboardTarget.performed += ctx => TrySetAnalogDirection(false, ctx.ReadValue<Vector2>());
        s_controls.PlayerCombat.KeyboardTarget.canceled += ctx => TrySetAnalogDirection(false, ctx.ReadValue<Vector2>());
        // Change powers
        if (m_allowPowerSelection)
        {
            s_controls.PlayerCombat.PowerSelection.performed += ctx => m_player.TryChangeEffect(ctx.ReadValue<Vector2>());
            s_controls.PlayerCombat.PowerRotation.performed += ctx => m_player.RotateCurrentPower(ctx.ReadValue<float>());
        }
        // Pause
        s_controls.PlayerMovement.Pause.performed += _ => m_player.Pause();
        s_controls.Pause.UnPause.performed += _ => m_player.UnPause();
        // Dialogue
        s_controls.Dialogue.Continue.performed += _ => m_player.ContinueDialogue();

        // Cheat console
        s_controls.Cheats.ToggleConsole.performed += _ => m_cheatConsole.OnToggleDebug();
        s_controls.Cheats.Return.performed += _ => m_cheatConsole.OnReturn();
        s_controls.Cheats.PreviousEntry.performed += _ => m_cheatConsole.PreviousEntry();
        s_controls.Cheats.NextEntry.performed += _ => m_cheatConsole.NextEntry();
        s_controls.Cheats.Enable();

        // Set init values
        SetMovement(m_defaultMovement);
        SetCombat(m_defaultCombat);
        s_controls.Pause.Disable();
        s_controls.Dialogue.Disable();
    }

    private void OnDestroy()
    {
        if (s_controls != null)
        {
            // Movement
            s_controls.PlayerMovement.Movement.performed -= ctx => TrySetAnalogDirection(true, ctx.ReadValue<Vector2>());
            s_controls.PlayerMovement.Movement.canceled -= ctx => TrySetAnalogDirection(true, ctx.ReadValue<Vector2>());
            s_controls.PlayerMovement.KeyboardMovement.performed -= ctx => TrySetAnalogDirection(true, ctx.ReadValue<Vector2>());
            s_controls.PlayerMovement.KeyboardMovement.canceled -= ctx => TrySetAnalogDirection(true, ctx.ReadValue<Vector2>());

            s_controls.PlayerMovement.Jump.performed -= _ => m_player.Jump();

            // Interact
            s_controls.PlayerMovement.Interact.performed -= _ => m_player.TryInteract();
            // Punch
            s_controls.PlayerCombat.Punch.performed -= _ => m_player.BeginPunchAnimation();
            // Raise Chunk
            s_controls.PlayerCombat.Raise.performed -= _ => m_player.BeginRaiseAnimation();
            // Target
            //s_controls.PlayerCombat.Target.performed -= ctx => TrySetAnalogDirection(false, ctx.ReadValue<Vector2>());
            //s_controls.PlayerCombat.Target.canceled -= ctx => TrySetAnalogDirection(false, ctx.ReadValue<Vector2>());
            s_controls.PlayerCombat.KeyboardTarget.performed -= ctx => TrySetAnalogDirection(false, ctx.ReadValue<Vector2>());
            s_controls.PlayerCombat.KeyboardTarget.canceled -= ctx => TrySetAnalogDirection(false, ctx.ReadValue<Vector2>());
            // Change powers
            if (m_allowPowerSelection)
            {
                s_controls.PlayerCombat.PowerSelection.performed -= ctx => m_player.TryChangeEffect(ctx.ReadValue<Vector2>());
                s_controls.PlayerCombat.PowerRotation.performed -= ctx => m_player.RotateCurrentPower(ctx.ReadValue<float>());
            }
            // Pause
            s_controls.PlayerMovement.Pause.performed -= _ => m_player.Pause();
            s_controls.Pause.UnPause.performed -= _ => m_player.UnPause();
            // Dialogue
            s_controls.Dialogue.Continue.performed -= _ => m_player.ContinueDialogue();

            // Cheat console
            s_controls.Cheats.ToggleConsole.performed -= _ => m_cheatConsole.OnToggleDebug();
            s_controls.Cheats.Return.performed -= _ => m_cheatConsole.OnReturn();
            s_controls.Cheats.PreviousEntry.performed -= _ => m_cheatConsole.PreviousEntry();
            s_controls.Cheats.NextEntry.performed -= _ => m_cheatConsole.NextEntry();

            s_controls.Disable();
        }
    }

    // Tries to set the analog stick direction. Will return instantly if the player is null
    private void TrySetAnalogDirection(bool _leftAnalog, Vector2 _direction)
    {
        if (!m_player)
        {
            return;
        }

        // Can only set Left analog stick
        m_player.SetLAnalogDirection(_direction);
        
        // Right
        //m_player.SetRAnalogDirection(_direction);
    }

    public void SetMovement(bool _active)
    {
        if (_active)
        {
            s_controls.PlayerMovement.Enable(); 
        }
        else
        {
            s_controls.PlayerMovement.Disable(); 
        }

        UpdateJump();
    }
    
    public bool HasMovement()
    {
        return s_controls.PlayerMovement.enabled;
    }

    public void SetCombat(bool _active)
    {
        if (_active)
        {
            s_controls.PlayerCombat.Enable(); 
        }
        else
        {
            s_controls.PlayerCombat.Disable(); 
        }
    }

    // Disables / enables jump based on bool status
    private void UpdateJump()
    {
        if (m_canJump)
        {
            s_controls.PlayerMovement.Jump.Enable();
        }
        else
        {
            s_controls.PlayerMovement.Jump.Disable();
        }
    }

    public void ToggleJump()
    {
        m_canJump = !m_canJump;
        UpdateJump();
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

    // Used in tutorial to disable only punching
    public void OverridePunching(bool _active)
    {
        if (_active)
        {
            s_controls.PlayerCombat.Punch.Enable();
            return;
        }

        s_controls.PlayerCombat.Punch.Disable();
    }
}
