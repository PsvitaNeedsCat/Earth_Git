using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using Cinemachine;

public class RoomManager : MonoBehaviour
{
    [HideInInspector] public Vector3 m_respawnLocation;

    [Tooltip("Room parent objects. First room in list is the first room in scene")]
    [SerializeField] private List<GameObject> m_rooms = new List<GameObject>();
    private int m_currentRoom = 0;
    private int m_newRoom;
    private Animator m_blackWall;
    private PlayerInput m_playerInput;
    private GameObject[] m_roomCopies;
    private GameObject m_camTarget;

    private bool m_loadScene = false;
    private string m_newScene = "";

    private static RoomManager s_instance;
    public static RoomManager Instance
    {
        get
        {
            return s_instance; 
        }
    }

    public int GetCurrentRoom()
    {
        return m_currentRoom; 
    }

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(this.gameObject); 
        }
        else 
        {
            s_instance = this; 
        }

        BlackWallAnimator blackWall = FindObjectOfType<BlackWallAnimator>();
        Debug.Assert(blackWall, "TURN THE BLACK WALL BACK ON");
        m_blackWall = blackWall.GetComponent<Animator>();
        m_playerInput = FindObjectOfType<PlayerInput>();
        Debug.Assert(m_playerInput, "Cannot find player input");

        // Disbale all rooms
        for (int i = 0; i < m_rooms.Count; i++)
        {
            m_rooms[i].SetActive(false);
        }

        // Create copies of the rooms
        m_roomCopies = new GameObject[m_rooms.Count];
        for (int i = 0; i < m_rooms.Count; i++)
        {
            m_roomCopies[i] = Instantiate(m_rooms[i], Vector3.zero, Quaternion.identity);
            m_roomCopies[i].SetActive(false);
        }

        // Find cam look target
        m_camTarget = GameObject.Find("CamTarget");

        // Activate the first room
        for (int i = 0; i < m_rooms.Count; i++)
        {
            if (i == m_currentRoom)
            {
                m_rooms[i].SetActive(true);
                break;
            }
        }
    }

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.fadedToBlack, ChangeRooms);
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.fadedToBlack, ChangeRooms);
    }

    // Changes which room is active - called by blackwall animator
    private void ChangeRooms(string _null)
    {
        // Change scene
        if (m_loadScene)
        {
            SceneManager.LoadScene(m_newScene);
        }
        // Change room
        else
        {
            // Deactivate old room
            m_rooms[m_currentRoom].SetActive(false);

            // Activate new room
            m_currentRoom = m_newRoom;
            m_rooms[m_currentRoom].SetActive(true);
            m_playerInput.transform.position = m_respawnLocation;

            m_blackWall.SetTrigger("FadeToGame");
            m_playerInput.SetMovement(true);
        }
    }

    // Sets the room ready to change to
    public void PrepareToChangeRoom(string _roomName)
    {
        // Check the room is valid
        for (int i = 0; i < m_rooms.Count; i++)
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

    // Loads a new scene
    public void LoadScene(string _sceneName)
    {
        m_loadScene = true;
        m_newScene = _sceneName;

        m_blackWall.SetTrigger("FadeToBlack");
    }

    // Resets the current room - used when player dies
    public void ReloadCurrentRoom()
    {
        string roomName = m_rooms[m_currentRoom].name;

        Vector3 oldPos = m_rooms[m_currentRoom].transform.position;

        // Destroy current room
        Destroy(m_rooms[m_currentRoom]);
        m_rooms.RemoveAt(m_currentRoom);

        // Spawn room
        GameObject newRoom = Instantiate(m_roomCopies[m_currentRoom], oldPos, Quaternion.identity);
        newRoom.name = roomName; // Set name so it isn't name(Clone)
        m_rooms.Insert(m_currentRoom, newRoom);
        m_rooms[m_currentRoom].SetActive(true);

        // Set camera to look at player
        if (m_currentRoom < m_rooms.Count - 1)
        {
            CinemachineVirtualCamera cam = m_rooms[m_currentRoom].GetComponentInChildren<CinemachineVirtualCamera>();
            if (cam)
            {
                cam.Follow = m_camTarget.transform;
            }
        }
    }

    // Loads the player into a specified room - also teleports player to the room
    // Skips fade in/out
    public void ForceLoadRoom(int _room)
    {
        // Validate
        if (_room >= m_rooms.Count || _room < 0)
        {
            Debug.Log("Room number " + _room + " is invalid");
            return;
        }

        // Disable all rooms except main one
        for (int i = 0; i < m_rooms.Count; i++)
        {
            m_rooms[i].SetActive(i == _room);
        }

        // If first room, leave player position
        if (_room == 0)
        {
            return;
        }

        // Get spawn point
        Vector3 spawnPoint = Vector3.zero;
        bool spawnFound = false;
        RoomTrigger[] triggers = FindObjectsOfType<RoomTrigger>();
        foreach (RoomTrigger i in triggers)
        {
            if (i.m_spawnPoint)
            {
                spawnPoint = i.transform.position;
                spawnPoint += i.m_spawnOffset;
                spawnFound = true;
                break;
            }
        }

        if (spawnFound)
        {
            // Set player spawn
            m_playerInput.transform.position = spawnPoint;

            m_currentRoom = _room;
        }
        else
        {
            // No spawn found
            // Load room prior
            Debug.Log("No spawn found, loading room prior");

            ForceLoadRoom(_room -= 1);
        }
    }

    // Fades to black without the animation calls
    public void FadeToBlack()
    {
        m_blackWall.SetTrigger("QuietFadeToBlack");
    }
    public void FadeToGame()
    {
        m_blackWall.SetTrigger("FadeToGame");
    }
}
