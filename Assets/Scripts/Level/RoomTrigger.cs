using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Tooltip("The name of the game object that holds all the room objects")]
    [SerializeField] private string m_roomName;

    private void OnTriggerEnter(Collider other)
    {
        // Collided with player
        Player player = other.GetComponent<Player>();
        if (player)
        {
            RoomManager.Instance.ChangeRooms(m_roomName);
        }
    }
}
