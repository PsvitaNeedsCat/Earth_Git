using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    static List<Chunk> m_chunks = new List<Chunk>();
    private const int m_maxChunks = 3;

    public static void AddChunk(Chunk _newChunk)
    {
        m_chunks.Add(_newChunk);
        if (m_chunks.Count > m_maxChunks)
        {
            RemoveOldest();
        }
    }
    public static void RemoveChunk(Chunk _removeChunk)
    {
        m_chunks.Remove(_removeChunk);
    }
    public static int NumChunks() => m_chunks.Count;

    public static void RemoveOldest()
    {
        if (m_chunks.Count > 0)
        {
            Destroy(m_chunks[0].gameObject);
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
        for (int i = 0; i < m_chunks.Count; i++)
        {
            Chunk chunk = m_chunks[i];

            if (chunk != null)
            {
                Destroy(chunk.gameObject);
            }
        }

        m_chunks.Clear();
    }
}
