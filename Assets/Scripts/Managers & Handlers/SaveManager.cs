using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    // Items that are saved when the player quits
    [System.Serializable]
    public class SaveFile
    {
        public string scene = "Dojo";
        public int room = 0;
        public Dictionary<EChunkEffect, bool> m_unlockedPowers = new Dictionary<EChunkEffect, bool>()
        {
            { EChunkEffect.none, true },
            { EChunkEffect.water, false },
            { EChunkEffect.fire, false },
            { EChunkEffect.mirage, false }
        };
        public int health = 3;
        public List<int> m_curCollectedKeys = new List<int>(); // Currently collected keys
        public bool[] m_unlockedDoors = null;
        public Dictionary<int, bool> m_prevCollectedKeys = new Dictionary<int, bool>(); // Previously collected keys
        public int m_lastTempleEntered = 0;
    }

    // 3 player saves
    private SaveFile[] m_saves = new SaveFile[3] { null, null, null };
    private int m_currentFile;

    private GlobalPlayerSettings m_settings;
    BinaryFormatter m_formatter = new BinaryFormatter();
    private bool m_initLoad = false;

    private static SaveManager s_instance;
    public static SaveManager Instance
    {
        get 
        {
            return s_instance; 
        }
    }

    private void Awake()
    {
        if (s_instance != null && s_instance != this) { Destroy(this.gameObject); }
        else { s_instance = this; }

        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");

        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += SceneLoaded;
    }

    // Saves the currently open save file
    public void SaveGame()
    {
        // Save to array
        m_saves[m_currentFile].scene = SceneManager.GetActiveScene().name;
        m_saves[m_currentFile].room = (RoomManager.Instance) ? RoomManager.Instance.GetCurrentRoom() : 0;
        m_saves[m_currentFile].m_unlockedPowers = Player.s_activePowers;
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player)
        {
            m_saves[m_currentFile].health = player.GetCurrentHealth();
            m_saves[m_currentFile].m_curCollectedKeys = player.GetComponent<Player>().m_collectedKeys;
            m_saves[m_currentFile].m_lastTempleEntered = Player.m_lastTempleEntered;
        }
        else 
        {
            Debug.LogError("Unbale to find player, could not save health"); 
        }
        DoorManager doorManager = FindObjectOfType<DoorManager>();
        if (doorManager)
        {
            m_saves[m_currentFile].m_unlockedDoors = doorManager.m_isDoorUnlocked;
            m_saves[m_currentFile].m_prevCollectedKeys = doorManager.m_collectedKeys;
        }
        else
        {
            m_saves[m_currentFile].m_unlockedDoors = null;
        }

        // Save to txt file
        FileStream fs = File.Open(Application.dataPath + "/" + m_settings.m_saveFileName + m_currentFile.ToString() + ".txt", FileMode.OpenOrCreate);
        m_formatter.Serialize(fs, m_saves[m_currentFile]);
        fs.Close();

        Debug.Log("Save game " + m_currentFile + " saved");
    }

    // Loads a specific save file
    public bool LoadGame(int _saveId)
    {
        UpdateSaves();

        // If the save does not exist
        if (m_saves[_saveId] == null)
        {
            return false; 
        }

        m_currentFile = _saveId;

        Debug.Log("Save file " + _saveId + " loaded");

        LoadSaveScene();

        return true;
    }

    // Updates the array with the saved .txt files
    private void UpdateSaves()
    {
        // Cheak each save
        for (int i = 0; i < m_saves.Length; i++)
        {
            string filePath = Application.dataPath + "/" + m_settings.m_saveFileName + i.ToString() + ".txt";

            if (File.Exists(filePath))
            {
                // Load the data
                FileStream fs = File.OpenRead(filePath);
                SaveFile save = (SaveFile)m_formatter.Deserialize(fs);
                fs.Close();

                // Check the save file is valid
                if (save == null)
                {
                    Debug.LogError("Save file " + m_currentFile + " is invalid");
                    m_saves[i] = null;
                    break;
                }

                m_saves[i] = save;
            }
            else
            {
                m_saves[i] = null; 
            }
        }
    }

    // Creates a new save file - does not save it
    public void CreateSave(int _saveId)
    {
        // Create a save
        m_currentFile = _saveId;
        m_saves[m_currentFile] = new SaveFile();

        Debug.Log("New save file created with an ID " + m_currentFile);

        LoadSaveScene();
    }

    // Loads the scene, room, and power of the current save
    private void LoadSaveScene()
    {
        m_initLoad = true;

        // Load the correct scene
        //SceneManager.LoadScene(m_saves[m_currentFile].scene);
        //StartCoroutine(LoadSceneAsync(m_saves[m_currentFile].scene));
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        SceneManager.LoadSceneAsync(m_saves[m_currentFile].scene);

        // Load the unlocked powers
        Player.s_activePowers = m_saves[m_currentFile].m_unlockedPowers;
        Player.s_currentEffect = EChunkEffect.none;
    }

    private void SceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        // Load the correct room
        if (m_initLoad && RoomManager.Instance)
        {
            m_initLoad = false;
            RoomManager.Instance.ForceLoadRoom(m_saves[m_currentFile].room);

            // Load the correct health
            Player player = FindObjectOfType<Player>();
            if (player)
            {
                player.GetComponent<HealthComponent>().Health = m_saves[m_currentFile].health;
                player.m_collectedKeys = m_saves[m_currentFile].m_curCollectedKeys;
                Player.m_lastTempleEntered = m_saves[m_currentFile].m_lastTempleEntered;
                player.InitLoad();
            }
            else
            {
                Debug.LogError("Unable to find player, could not load health"); 
            }
            if (m_saves[m_currentFile].m_unlockedDoors != null)
            {
                DoorManager doorManager = FindObjectOfType<DoorManager>();
                doorManager.Init(m_saves[m_currentFile].m_unlockedDoors, m_saves[m_currentFile].m_prevCollectedKeys);
            }

            MessageBus.TriggerEvent(EMessageType.checkKeyID);
        }
    }

    // Returns a save file - used for displaying info
    public SaveFile GetSaveFile(int _saveId)
    {
        if (_saveId >= m_saves.Length)
        {
            return null; 
        }

        UpdateSaves();

        return m_saves[_saveId];
    }

    // Deletes a save file at a given ID
    public void DeleteSave(int _saveId)
    {
        string filePath = Application.dataPath + "/" + m_settings.m_saveFileName + _saveId.ToString() + ".txt";

        // Check that there is a save file
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            UpdateSaves();
        }
        else
        {
            Debug.LogError("Cannot delete save file " + _saveId); 
        }
    }
}
