using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Public variables

    // Private variables
    [SerializeField] private GlobalTileSettings m_globalSettings;
    private eType m_type;

    // Returns null if chunk failed to raise
    public Chunk TryRaiseChunk()
    {
        Chunk newChunk = null;

        // Cannot raise a type of none
        if (m_type == eType.none) { return newChunk; }

        // Cannot raise an occupied tile
        if (IsOccupied()) { return newChunk; }

        // Spawn new chunk and raise it
        GameObject chunkPrefab = m_globalSettings.m_chunkPrefabs[(int)m_type];
        newChunk = Instantiate(chunkPrefab, transform.position, transform.rotation, null).GetComponent<Chunk>();
        newChunk.RaiseChunk();

        return newChunk;
    }

    // Checks if there is a chunk currently above thise tile
    public bool IsOccupied()
    {
        return Physics.Raycast(transform.position, Vector3.up, m_globalSettings.m_tileSize, m_globalSettings.m_raycastMask);
    }

    // Returns the tile's type
    public eType GetTileType()
    {
        return m_type;
    }
}
