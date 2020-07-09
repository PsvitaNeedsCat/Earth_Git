using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    // Items that are saved when the player quits
    [System.Serializable]
    public class SaveFile
    {
        public string scene = "Overworld";
        public int room = 0;
        public Dictionary<eChunkEffect, bool> unlockedPowers = new Dictionary<eChunkEffect, bool>()
        {
            { eChunkEffect.none, true },
            { eChunkEffect.water, false },
            { eChunkEffect.fire, false }
        };
    }

    // 3 player saves
    private SaveFile[] m_saves = new SaveFile[3] { null, null, null };
    private int m_currentFile;

    private GlobalPlayerSettings m_settings;
    BinaryFormatter m_formatter = new BinaryFormatter();
    private bool m_initLoad = false;

    private static SaveManager m_instace;
    public static SaveManager Instance { get { return m_instace; } }

    private void Awake()
    {
        if (m_instace != null && m_instace != this) { Destroy(this.gameObject); }
        else { m_instace = this; }

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
        m_saves[m_currentFile].unlockedPowers = Player.m_activePowers;

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
        if (m_saves[_saveId] == null) { return false; }

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
            else { m_saves[i] = null; }
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
        SceneManager.LoadScene(m_saves[m_currentFile].scene);

        // Load the unlocked powers
        Player.m_activePowers = m_saves[m_currentFile].unlockedPowers;
    }

    private void SceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        // Load the correct room
        if (m_initLoad && RoomManager.Instance)
        {
            m_initLoad = false;
            RoomManager.Instance.ForceLoadRoom(m_saves[m_currentFile].room); 
        }
    }

    // Gets a save file - used for displaying info
    public SaveFile GetSaveFile(int _saveId)
    {
        if (_saveId >= m_saves.Length) { return null; }

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
        else { Debug.LogError("Cannot delete save file " + _saveId); }
    }
}
