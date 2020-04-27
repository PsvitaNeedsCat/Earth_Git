using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void AEPunch()
    {
        // player.Punch();
    }

    public void AEEnableMovement()
    {
        // player.EnableMovement();
    }

    public void AEDisableMovement()
    {
        // player.DisableMovement();
    }
}
