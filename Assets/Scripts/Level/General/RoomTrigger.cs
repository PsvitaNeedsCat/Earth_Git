using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
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
            RoomManager.Instance.m_respawnLocation = transform.position;

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
