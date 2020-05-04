using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneDatabase : MonoBehaviour
{
    class ObjectData
    {
        public string m_name;
        public Vector3 m_position;
        public Quaternion m_rotation;
        public bool m_active;
        public eChunkType m_chunkType = eChunkType.none;
    }

    private static SceneDatabase m_instance;
    public static SceneDatabase Instance { get { return m_instance; } }

    private Dictionary<string, List<ObjectData>> m_database = new Dictionary<string, List<ObjectData>>();

    private GlobalTileSettings m_tileSettings;

    private void Awake()
    {
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }

        DontDestroyOnLoad(this.gameObject);

        m_tileSettings = Resources.Load<GlobalTileSettings>("ScriptableObjects/GlobalTileSettings");

        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnEnable()
    {
        // Add listeners
        MessageBus.AddListener(EMessageType.spittingEnemyDestroyed, EnemyDestroyed);
    }

    private void OnDisable()
    {
        // Remove listeners
        MessageBus.RemoveListener(EMessageType.spittingEnemyDestroyed, EnemyDestroyed);
    }

    // Will be called when a new scene is successfully loaded
    private void SceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        ChunkManager.m_changingScene = false;

        // If level isn't in database; add it to the database
        if (!m_database.ContainsKey(_scene.name))
        {
            AddToDatabse(_scene.name);
        }
        // Level is in database; load it from database
        else
        {
            LoadFromDatabse(_scene.name);
        }
    }

    public void LoadScene(string _name)
    {
        // Save this scene's data
        SaveSceneData(SceneManager.GetActiveScene().name);

        ChunkManager.m_changingScene = true;

        // Load new scene
        SceneManager.LoadScene(_name);
    }

    private void SaveSceneData(string _name)
    {
        // Update transforms for objects
        for (int i = 0; i < m_database[_name].Count; i++)
        {
            ObjectData current = m_database[_name][i];

            GameObject currentObject = GameObject.Find(current.m_name);

            // If the object still exists
            if (currentObject)
            {
                current.m_position = currentObject.transform.position;
                current.m_rotation = currentObject.transform.rotation;

                // If object is a chunk and it is not fully raised, remove it
                Chunk chunk = currentObject.GetComponent<Chunk>();
                if (chunk && !chunk.m_isRaised)
                {
                    m_database[_name].RemoveAt(i);
                }
            }

            current.m_active = currentObject;
        }
    }

    private void AddToDatabse(string _name)
    {
        List<ObjectData> data = new List<ObjectData>();

        // Save all object data
        // To be added
        // for (int i = 0; i < FindObjectsOfType<SpittingEnemy>().Length; i++)
        // Create ObjectData and add to data
        foreach (SpittingEnemy i in FindObjectsOfType<SpittingEnemy>())
        {
            ObjectData newData = new ObjectData();
            newData.m_name = i.name;
            newData.m_position = i.transform.position;
            newData.m_rotation = i.transform.rotation;
            newData.m_active = true;

            data.Add(newData);
        }

        // Add to database
        m_database.Add(_name, data);
    }

    private void LoadFromDatabse(string _name)
    {
        // Load everything in databse for this scene
        for (int i = 0; i < m_database[_name].Count; i++)
        {
            ObjectData currentCheck = m_database[_name][i];

            GameObject currentObject = GameObject.Find(currentCheck.m_name);
            if (currentObject && !currentCheck.m_active)
            {
                // Object exists and is not meant to
                Destroy(currentObject);
            }
            else if (!currentObject && currentCheck.m_active)
            {
                // Object does not exist but it is supposed to
                GameObject prefab = m_tileSettings.m_chunkPrefabs[(int)currentCheck.m_chunkType];
                GameObject newChunk = Instantiate(prefab, currentCheck.m_position, currentCheck.m_rotation);
                newChunk.name = currentCheck.m_name;
                newChunk.GetComponent<Chunk>().m_isRaised = true;
            }
            else
            {
                // Exists and is meant to
                // Update position and rotation
                currentObject.transform.position = currentCheck.m_position;
                currentObject.transform.rotation = currentCheck.m_rotation;
            }
        }
    }

    public void AddChunk(Chunk _chunk)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Check that it doesn't already exist
        for (int i = 0; i < m_database[currentScene].Count; i++)
        {
            // It has a chunk type of not none (it is a chunk)
            // & the position matches
            if (m_database[currentScene][i].m_chunkType != eChunkType.none && 
                m_database[currentScene][i].m_position == _chunk.transform.position)
            {
                // Chunk exists
                return;
            }
        }

        ObjectData newData = new ObjectData();
        newData.m_name = _chunk.name;
        newData.m_active = true;
        newData.m_position = _chunk.transform.position;
        newData.m_rotation = _chunk.transform.rotation;
        newData.m_chunkType = _chunk.m_chunkType;

        m_database[SceneManager.GetActiveScene().name].Add(newData);
    }

    public void RemoveChunk(Chunk _chunk)
    {
        // Check all object data in current scene
        for (int i = 0; i < m_database[SceneManager.GetActiveScene().name].Count; i++)
        {
            // If the names match
            if (m_database[SceneManager.GetActiveScene().name][i].m_name == _chunk.name)
            {
                m_database[SceneManager.GetActiveScene().name].RemoveAt(i);
                return;
            }
        }

        Debug.LogError("Unable to remove chunk from scene database");
    }

    private void EnemyDestroyed(string _name)
    {
        // For every object in current scene
        foreach (ObjectData i in m_database[SceneManager.GetActiveScene().name])
        {
            if (i.m_name == _name)
            {
                i.m_active = false;
                return;
            }
        }

        Debug.LogError("Could not find enemy with name: " + _name);
    }
}
