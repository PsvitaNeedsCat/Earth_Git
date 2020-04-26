﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Tile : MonoBehaviour
{
    // Public variables

    // Private variables
    GlobalTileSettings m_globalSettings;
    eChunkType m_chunkType;

    // Tiles automatically added to and removed from grid over lifetime
    private void OnEnable() => Grid.AddTile(this);
    private void OnDisable() => Grid.RemoveTile(this);

    private void Awake()
    {
        m_globalSettings = Resources.Load<GlobalTileSettings>("ScriptableObjects/GlobalTileSettings"); ;
    }

    // Returns null if chunk failed to raise
    public Chunk TryRaiseChunk()
    {
        Chunk newChunk = null;

        // Cannot raise a type of none
        if (m_chunkType == eChunkType.none) { return newChunk; }

        // Cannot raise an occupied tile
        if (IsOccupied()) { return newChunk; }

        // Spawn new chunk and raise it
        GameObject chunkPrefab = m_globalSettings.m_chunkPrefabs[(int)m_chunkType];
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
    public eChunkType GetTileType()
    {
        return m_chunkType;
    }
}