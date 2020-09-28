using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTeleporter : Interactable
{
    [Tooltip("The name of the room where the player will be teleported")]
    [SerializeField] private string m_destinationRoom = "";
    private bool m_unlocked = false;

    // Stops the player moving and teleports them back to a specified room
    public override void Invoke()
    {
        if (!m_unlocked)
        {
            return;
        }

        MessageBus.TriggerEvent(EMessageType.crystalHealed);

        PlayerInput player = FindObjectOfType<PlayerInput>();

        player.SetMovement(false);

        FindObjectOfType<RoomManager>().PrepareToChangeRoom(m_destinationRoom, false);
    }

    // Stop prompt from appearing if locked
    public override void Update()
    {
        if (m_unlocked)
        {
            base.Update();
        }
    }

    // Unlocks the teleport for use
    public void Unlock()
    {
        m_unlocked = true;
    }
}
