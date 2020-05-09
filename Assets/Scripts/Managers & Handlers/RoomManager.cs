using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Tooltip("Room parent objects. First room in list is the first room in scene")]
    [SerializeField] private GameObject[] m_rooms;
    private int m_currentRoom = 0;

    private static RoomManager m_instance;
    public static RoomManager Instance { get { return m_instance; } }

    private void Awake()
    {
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }

        for (int i = 0; i < m_rooms.Length; i++)
        {
            m_rooms[i].SetActive(i == m_currentRoom);
        }
    }

    // Changes which room is active
    public void ChangeRooms(string _roomName)
    {
        // Find room index
        for (int i = 0; i < m_rooms.Length; i++)
        {
            if (m_rooms[i].name == _roomName)
            {
                // Do not change room is _roomName is the current room
                if (m_currentRoom == i) { return; }

                // Deacivate old room
                m_rooms[m_currentRoom].SetActive(false);

                // Activate new room
                m_currentRoom = i;
                m_rooms[m_currentRoom].SetActive(true);

                return;
            }
        }

        Debug.LogError("Cannot find room with name: " + _roomName);
    }

    // Returns the active room
    public GameObject GetActiveRoom()
    {
        return m_rooms[m_currentRoom];
    }
}
