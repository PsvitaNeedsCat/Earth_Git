using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    private static List<Chunk> s_chunks = new List<Chunk>();
    private const int m_maxChunks = 3;

    public static void AddChunk(Chunk _newChunk)
    {
        s_chunks.Add(_newChunk);
        if (s_chunks.Count > m_maxChunks)
        {
            RemoveOldest();
        }
    }
    public static void RemoveChunk(Chunk _removeChunk)
    {
        s_chunks.Remove(_removeChunk);
    }
    public static int NumChunks() => s_chunks.Count;

    public static void RemoveOldest()
    {
        if (s_chunks.Count > 0)
        {
            s_chunks[0].OnDeath();
        }
    }

    private ChunkManager m_instance;

    private void Awake()
    {
        // Only one instance of this
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }
    }

    public static void DestroyAllChunks()
    {
        for (int i = 0; i < s_chunks.Count;)
        {
            Chunk chunk = s_chunks[i];

            if (chunk != null)
            {
                //Destroy(chunk.gameObject);
                chunk.GetComponent<HealthComponent>().Health = 0;
            }
            else
            {
                i++;
            }
        }

        s_chunks.Clear();
    }
}
