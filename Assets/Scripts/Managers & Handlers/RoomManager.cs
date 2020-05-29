using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [Tooltip("Room parent objects. First room in list is the first room in scene")]
    [SerializeField] private GameObject[] m_rooms;
    private int m_currentRoom = 0;
    private int m_newRoom;
    private Animator m_blackWall;
    private PlayerInput m_playerInput;

    private bool m_loadScene = false;
    private string m_newScene = "";

    private static RoomManager m_instance;
    public static RoomManager Instance { get { return m_instance; } }

    private void Awake()
    {
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }

        m_blackWall = FindObjectOfType<BlackWallAnimator>().GetComponent<Animator>();
        Debug.Assert(m_blackWall, "Cannot find black wall animator");
        m_playerInput = FindObjectOfType<PlayerInput>();
        Debug.Assert(m_playerInput, "Cannot find player input");

        for (int i = 0; i < m_rooms.Length; i++)
        {
            m_rooms[i].SetActive(i == m_currentRoom);
        }
    }

    // Changes which room is active
    public void ChangeRooms()
    {
        // Change scene
        if (m_loadScene)
        {
            SceneManager.LoadScene(m_newScene);
        }
        // Change room
        else
        {
            // Deacivate old room
            m_rooms[m_currentRoom].SetActive(false);

            // Activate new room
            m_currentRoom = m_newRoom;
            m_rooms[m_currentRoom].SetActive(true);

            m_blackWall.SetTrigger("FadeToGame");
            m_playerInput.SetMovement(true);
        }
    }

    // Sets the room ready to change to
    public void PrepareToChangeRoom(string _roomName)
    {
        // Check the room is valid
        for (int i = 0; i < m_rooms.Length; i++)
        {
            if (m_rooms[i].name == _roomName)
            {
                // Cannot load the same room
                if (i == m_currentRoom) { return; }

                // Room is valid
                m_playerInput.SetMovement(false);
                m_newRoom = i;
                m_blackWall.SetTrigger("FadeToBlack");
                return;
            }
        }

        Debug.LogError("Room does not exist");
    }

    // Returns the active room
    public GameObject GetActiveRoom()
    {
        return m_rooms[m_currentRoom];
    }

    public void LoadScene(string _sceneName)
    {
        m_loadScene = true;
        m_newScene = _sceneName;

        m_blackWall.SetTrigger("FadeToBlack");
    }
}
