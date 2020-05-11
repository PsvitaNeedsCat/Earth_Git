using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eChunkType
{
    none,
    rock,
    poison,
    lava
}

[CreateAssetMenu(fileName = "NewGlobalTileSettings", menuName = "Settings/GlobalTileSettings")]
public class GlobalTileSettings : ScriptableObject
{
    public float m_tileSize = 10.0f;

    [Tooltip("Layer mask used when checking for chunks above tile")]
    public LayerMask m_raycastMask;

    [Tooltip("Chunk prefabs in the order of eType" +
        "null, rock, posion")]
    public GameObject[] m_chunkPrefabs;

    [Tooltip("How much force the lava will use to push the player back")]
    public float m_lavaPushForce = 10.0f;
}
