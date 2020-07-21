using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Public variables

    // Private variables
    GlobalTileSettings m_globalSettings;
    [SerializeField] eChunkType m_chunkType;
    private Collider m_collider;
    private bool m_ignore = false;

    // Tiles automatically added to and removed from grid over lifetime
    private void OnEnable() => Grid.AddTile(this);
    private void OnDisable() => Grid.RemoveTile(this);

    private void Awake()
    {
        m_globalSettings = Resources.Load<GlobalTileSettings>("ScriptableObjects/GlobalTileSettings");
        m_collider = GetComponentInChildren<Collider>();
    }

    // Returns null if chunk failed to raise
    public Chunk TryRaiseChunk()
    {
        Chunk newChunk = null;

        // Cannot raise a type of none
        if (m_chunkType == eChunkType.none || m_chunkType == eChunkType.lava) { return newChunk; }

        // Cannot raise an occupied tile
        if (IsOccupied()) { return newChunk; }

        // Spawn new chunk and raise it
        GameObject chunkPrefab = m_globalSettings.m_chunkPrefabs[(int)m_chunkType];
        newChunk = Instantiate(chunkPrefab, transform.position, Quaternion.identity, null).GetComponent<Chunk>();
        newChunk.m_chunkType = m_chunkType;
        newChunk.RaiseChunk();

        MessageBus.TriggerEvent(EMessageType.chunkRaise);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.small);
        EffectsManager.SpawnEffect(EffectsManager.EEffectType.rockSummon, newChunk.transform.position, Quaternion.identity, newChunk.transform.localScale, 2.0f);

        return newChunk;
    }

    // Checks if there is a chunk currently above thise tile
    public bool IsOccupied()
    {
        if (m_ignore)
        {
            return true;
        }
        Vector3 centre = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        return Physics.CheckBox(centre, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, m_globalSettings.m_raycastMask);
    }

    // Returns the tile's type
    public eChunkType GetTileType()
    {
        return m_chunkType;
    }

    public void SetChunkType(eChunkType _type)
    {
        m_chunkType = _type;
    }

    public void SetCollider(bool _active)
    {
        m_collider.enabled = _active;
    }

    public void SetIgnore(bool _ignore)
    {
        m_ignore = _ignore;
    }
}
