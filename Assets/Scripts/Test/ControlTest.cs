using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTest : MonoBehaviour
{
    bool movement = false;
    bool combat = false;

    public void MovementToggle()
    {
        movement = !movement;
        FindObjectOfType<PlayerInput>().SetMovement(movement);
    }

    public void CombatToggle()
    {
        combat = !combat;
        FindObjectOfType<PlayerInput>().SetCombat(combat);
    }
}
