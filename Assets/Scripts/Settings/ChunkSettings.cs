using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChunkSettings", menuName = "Settings/ChunkSettings")]
public class ChunkSettings : ScriptableObject
{
    [Header("Chunk abilities")]
    [Tooltip("What is the base element of this chunk")]
    public eChunkType m_type = eChunkType.rock;

    [Tooltip("How much damage the chunk does to enemies")]
    public float m_damage = 1;

    [Tooltip("How much damage the chunk takes before being destroyed")]
    public int m_maxHealth = 2;
}
