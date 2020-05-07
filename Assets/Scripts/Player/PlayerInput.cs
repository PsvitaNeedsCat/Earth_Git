using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Public variables

    // Private variables
    private InputMaster m_controls;
    private Player m_player;

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
        m_controls.PlayerMovement.Movement.performed += ctx => m_player.m_moveDirection = ctx.ReadValue<Vector2>();
        m_controls.PlayerMovement.Movement.canceled += ctx => m_player.m_moveDirection = ctx.ReadValue<Vector2>();
        // Punch
        m_controls.PlayerCombat.Punch.performed += _ => m_player.StartPunchAnim();
        // Raise Chunk
        m_controls.PlayerCombat.Raise.performed += _ => m_player.ActivateTileTargeter();
        m_controls.PlayerCombat.Raise.canceled += _ => m_player.StartRaiseChunkAnim();

        // Enable by default for now
        SetMovement(true);
        SetCombat(true);
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
}
