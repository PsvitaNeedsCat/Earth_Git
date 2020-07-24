using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerControl : MonoBehaviour
{
    [SerializeField] private bool m_hasCombatControls = true;
    [SerializeField] private bool m_hasMovementControls = true;

    private void Start()
    {
        PlayerInput player = FindObjectOfType<PlayerInput>();

        player.SetMovement(m_hasMovementControls);
        player.SetCombat(m_hasCombatControls);

        gameObject.SetActive(false);
    }
}
