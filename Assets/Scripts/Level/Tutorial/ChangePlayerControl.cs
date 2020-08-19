using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerControl : MonoBehaviour
{
    [SerializeField] private bool m_playerHasCombat = true;
    [SerializeField] private bool m_playerHasMovement = true;

    private void Start()
    {
        PlayerInput player = FindObjectOfType<PlayerInput>();
        if (player)
        {
            player.SetCombat(m_playerHasCombat);
            player.SetMovement(m_playerHasMovement);
        }
    }
}
