using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Player m_player;
    private PlayerInput m_playerInput;
    private PlayerController m_playerController;

    private void Awake()
    {
        m_playerInput = GetComponentInParent<PlayerInput>();
        m_player = GetComponentInParent<Player>();
        m_playerController = GetComponentInParent<PlayerController>();
    }

    public void AEPunch()
    {
        m_player.TryPunch();
    }

    public void AERaiseChunk()
    {
        m_player.TryRaiseChunk();
    }

    public void AEEnableMovement()
    {
        m_playerInput.SetMovement(true);
    }

    public void AEDisableMovement()
    {
        m_playerInput.SetMovement(false);
    }
}
