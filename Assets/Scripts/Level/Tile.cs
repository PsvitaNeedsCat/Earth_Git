using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Public variables


    // Private variables
    [SerializeField] private eType m_type;
    [SerializeField] private GlobalTileSettings m_globalSettings;

    public void TryRaiseChunk()
    {
        // Cannot raise a type of none
        if (m_type == eType.none) { return; }

        // Cannot raise an occupied tile
        if (IsOccupied()) { return; }

        // Spawn new chunk and raise it
        GameObject chunkPrefab = m_globalSettings.m_chunkPrefabs[(int)m_type];
        Chunk newChunk = Instantiate(chunkPrefab, transform.position, transform.rotation, null).GetComponent<Chunk>();
        newChunk.RaiseChunk();
    }

    // Checks if there is a chunk currently above thise tile
    public bool IsOccupied()
    {
        return Physics.Raycast(transform.position, Vector3.up, m_globalSettings.m_tileSize, m_globalSettings.m_raycastMask);
    }
}
