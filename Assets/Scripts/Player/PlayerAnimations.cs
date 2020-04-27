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

    }

    public void AEEnableMovement()
    {

    }

    public void AEDisableMovement()
    {

    }
}
