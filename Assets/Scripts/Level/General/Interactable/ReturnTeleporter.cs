using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTeleporter : Interactable
{
    [Tooltip("The name of the room where the player will be teleported")]
    [SerializeField] private string m_destinationRoom = "";

    // Stops the player moving and teleports them back to a specified room
    public override void Invoke()
    {
        PlayerInput player = FindObjectOfType<PlayerInput>();

        player.SetMovement(false);

        FindObjectOfType<RoomManager>().PrepareToChangeRoom(m_destinationRoom, false);
    }
}
