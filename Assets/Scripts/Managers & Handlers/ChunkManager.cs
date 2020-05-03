using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static bool m_changingScene = false;

    static List<Chunk> m_chunks = new List<Chunk>();

    public static void AddChunk(Chunk _newChunk)
    {
        m_chunks.Add(_newChunk);
    }
    public static int NumChunks() => m_chunks.Count;

    public static void RemoveChunk(Chunk _removeChunk)
    {
        if (!m_changingScene) { SceneDatabase.Instance.RemoveChunk(_removeChunk); }
        Destroy(_removeChunk.gameObject);
        m_chunks.Remove(_removeChunk);
    }

    public static void RemoveOldest()
    {
        if (m_chunks.Count > 0)
        {
            Destroy(m_chunks[0].gameObject);
            m_chunks.RemoveAt(0);
        }
    }

    private ChunkManager m_instance;

    private void Awake()
    {
        // Only one instance of this
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }
    }
}
