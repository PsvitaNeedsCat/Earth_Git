﻿using System.Collections;
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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    // Will be called when a new scene is successfully loaded
    private void SceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
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
            }

            current.m_active = currentObject;
        }
    }

    public void AddToDatabse(string _name)
    {
        List<ObjectData> data = new List<ObjectData>();

        // Save all cubes - test
        CheckCube[] cubes = FindObjectsOfType<CheckCube>();
        for (int i = 0; i < cubes.Length; i++)
        {
            ObjectData newData = new ObjectData();
            newData.m_name = cubes[i].name;
            newData.m_position = cubes[i].transform.position;
            newData.m_rotation = cubes[i].transform.rotation;
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
            else if (!currentObject)
            {
                // Check if object is a chunk
                Chunk chunkCheck = currentObject.GetComponent<Chunk>();
                if (chunkCheck)
                {
                    // If chunk, then it should exist, but it wont
                    GameObject prefab = m_tileSettings.m_chunkPrefabs[(int)chunkCheck.m_chunkType];
                    Instantiate(prefab, currentCheck.m_position, currentCheck.m_rotation);
                }
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
}
