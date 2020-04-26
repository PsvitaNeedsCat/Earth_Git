using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChunkManager : MonoBehaviour
{
    static List<Chunk> m_chunks = new List<Chunk>();

    public static void AddChunk(Chunk _newChunk) => m_chunks.Add(_newChunk);
    public static int NumChunks() => m_chunks.Count;

    public static void RemoveChunk(Chunk _removeChunk)
    {
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

    // Draw lines from manager to all chunks
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach(Chunk chunk in m_chunks)
        {
            Vector3 thisPos = transform.position;
            Vector3 chunkPos = chunk.transform.position;
            float halfHeight = (thisPos.y - chunkPos.y) / 2.0f;
            Vector3 offset = Vector3.up * halfHeight;

            Handles.DrawBezier(
                thisPos,
                chunkPos,
                thisPos - offset,
                chunkPos + offset,
                Color.white,
                EditorGUIUtility.whiteTexture,
                1.0f
                );
        }
    }
#endif
}
