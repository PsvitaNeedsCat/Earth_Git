using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerInput : MonoBehaviour
{
    // Private variables
    private InputMaster m_controls;
    private Player m_player;

    private bool m_prevMovement;
    private bool m_prevCombat;

    private bool m_isTargeting = false;

    // Only one instance
    private static PlayerInput m_instance;

    private void Awake()
    {
        // Only one instance of the player
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("A second instance of 'PlayerInput.cs' wasinstantiated");
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }

        // Init
        m_controls = new InputMaster();
        m_player = GetComponent<Player>();
        Debug.Assert(m_player, "No Player.cs found on player object");

        // Controls //

        // Movement
        m_controls.PlayerMovement.Movement.performed += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>(), m_isTargeting);
        m_controls.PlayerMovement.Movement.canceled += ctx => m_player.SetLAnalogDirection(ctx.ReadValue<Vector2>(), m_isTargeting);
        // Interact
        m_controls.PlayerMovement.Interact.performed += _ => m_player.TryInteract();
        // Punch
        m_controls.PlayerCombat.Punch.performed += _ => m_player.StartPunchAnim();
        // Raise Chunk
        m_controls.PlayerCombat.Raise.performed += _ => BeginTileTarget();
        m_controls.PlayerCombat.Raise.canceled += _ => EndTileTarget();
        // Change powers
        m_controls.PlayerCombat.NoPower.performed += _ => m_player.TryChangeEffect(eChunkEffect.none);
        m_controls.PlayerCombat.WaterPower.performed += _ => m_player.TryChangeEffect(eChunkEffect.water);
        m_controls.PlayerCombat.FirePower.performed += _ => m_player.TryChangeEffect(eChunkEffect.fire);
        // Pause
        m_controls.PlayerMovement.Pause.performed += _ => m_player.Pause();
        m_controls.Pause.UnPause.performed += _ => m_player.UnPause();
        // Dialogue
        m_controls.Dialogue.Continue.performed += _ => m_player.ContinueDialogue();

        // Enable by default for now
        SetMovement(true);
        SetCombat(true);
        m_controls.Pause.Disable();
        m_controls.Dialogue.Disable();
    }

    private void OnDestroy()
    {
        if (m_instance == this) { m_controls.Disable(); }
    }

    public void SetMovement(bool _active)
    {
        if (_active) { m_controls.PlayerMovement.Enable(); }
        else { m_controls.PlayerMovement.Disable(); }
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

    // Called when the player holds down A
    private void BeginTileTarget()
    {
        SetMovement(false);
        m_isTargeting = true;
        m_player.ActivateTileTargeter();
    }

    // Called when the player releases A
    private void EndTileTarget()
    {
        m_player.StartRaiseChunkAnim();
        m_player.DeactivateTileTargeter();
        SetMovement(true);
        m_isTargeting = false;
    }
}
