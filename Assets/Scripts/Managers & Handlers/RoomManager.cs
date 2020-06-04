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
    private GameObject[] m_roomPrefabs = null;
    private GameObject m_camTarget;

    private bool m_loadScene = false;
    private string m_newScene = "";

    private static RoomManager m_instance;
    public static RoomManager Instance { get { return m_instance; } }

    private void Awake()
    {
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }

        BlackWallAnimator blackWall = FindObjectOfType<BlackWallAnimator>();
        Debug.Assert(blackWall, "TURN THE BLACK WALL BACK ON");
        m_blackWall = blackWall.GetComponent<Animator>();
        m_playerInput = FindObjectOfType<PlayerInput>();
        Debug.Assert(m_playerInput, "Cannot find player input");

        for (int i = 0; i < m_rooms.Count; i++)
        {
            m_rooms[i].SetActive(i == m_currentRoom);
        }

        // Find cam look target
        m_camTarget = GameObject.Find("CamTarget");
    }

    private void Start() => MessageBus.AddListener(EMessageType.fadedToBlack, ChangeRooms);
    private void OnDestroy() => MessageBus.RemoveListener(EMessageType.fadedToBlack, ChangeRooms);

    // Changes which room is active - called by blackwall animator
    public void ChangeRooms(string _null)
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
        // If prefabs array is empty, populate it
        if (m_roomPrefabs == null)
        { m_roomPrefabs = Resources.LoadAll<GameObject>("Prefabs/Rooms/" + SceneManager.GetActiveScene().name); }

        string roomName = m_rooms[m_currentRoom].name;

        // Destroy current room
        Destroy(m_rooms[m_currentRoom]);
        m_rooms.RemoveAt(m_currentRoom);

        // Find the prefab with the same name 
        // (so that they don't have to be in the same order)
        GameObject prefab = null;
        for (int i = 0; i < m_roomPrefabs.Length; i++)
        {
            if (m_roomPrefabs[i].name == roomName)
            {
                prefab = m_roomPrefabs[i];
                break;
            }
        }

        // Spawn room
        GameObject newRoom = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        newRoom.name = roomName; // Set name so it isn't name(Clone)
        m_rooms.Insert(m_currentRoom, newRoom);

        // Set camera to look at player
        CinemachineVirtualCamera cam = m_rooms[m_currentRoom].GetComponentInChildren<CinemachineVirtualCamera>();
        cam.Follow = m_camTarget.transform;
    }

    // Fades to black without the animation calls
    public void FadeToBlack() => m_blackWall.SetTrigger("QuietFadeToBlack");
    public void FadeToGame() => m_blackWall.SetTrigger("FadeToGame");
}
