﻿using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Tooltip("Each room needs at least one, sets the position of spawn for the player when loading a save")]
    public bool m_spawnPoint = false;
    public Vector3 m_spawnOffset;

    [Tooltip("The name of the game object that holds all the room objects")]
    [SerializeField] private string m_roomName;
    [SerializeField] private EMessageType m_musicTrigger = EMessageType.none;
    [Tooltip("Unlocks this door when the trigger is collidede with")]
    [SerializeField] private GameObject m_unlockDoor = null;

    private void OnTriggerEnter(Collider other)
    {
        // Collided with player
        Player player = other.GetComponent<Player>();
        if (player)
        {
            // Set room manager
            RoomManager.Instance.PrepareToChangeRoom(m_roomName);
            RoomManager.Instance.m_respawnLocation = transform.position - new Vector3(0.0f, 0.45f, 0.0f);

            // Set music if applicable
            if (m_musicTrigger != EMessageType.none)
            {
                MessageBus.TriggerEvent(m_musicTrigger);
            }

            // Unlock door if applicable
            if (m_unlockDoor)
            {
                Destroy(m_unlockDoor);
            }
        }
    }
}
