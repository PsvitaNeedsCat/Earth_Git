using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlobalChunkSettings", menuName = "Settings/GlobalChunkSettings")]
public class GlobalChunkSettings : ScriptableObject
{
    [Tooltip("Is the application quitting")]
    [HideInInspector] public bool m_isQuitting = false;

    [Tooltip("How long it takes for the chunk to fully raise")]
    public float m_raiseTime = 0.5f;
    [Tooltip("How far the chunk will raise in units")]
    public float m_raiseAmount = 10.0f;

    [Tooltip("Maximum distance of raycast when checking for a wall." +
        "From the middle bottom of the chunk")]
    public float m_wallCheckDistance = 5.5f;

    [Tooltip("How tall the chunk is")]
    public float m_chunkHeight = 10.0f;

    [Tooltip("Layers that walls are on that the chunks should NOT collide with")]
    public LayerMask m_wallLayers;
}
