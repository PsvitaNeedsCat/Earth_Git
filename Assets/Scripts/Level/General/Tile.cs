using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Public variables
    public Material m_highlightedMaterial;

    // Private variables
    GlobalTileSettings m_globalSettings;
    [SerializeField] EChunkType m_chunkType;
    [Tooltip("True if this is the tile the player should spawn at when teleporting")]
    [SerializeField] private bool m_teleportTile = false;
    private Collider m_collider;
    private bool m_ignore = false;
    private Material m_normalMaterial;
    private Texture m_normalTexture;
    private MeshRenderer m_renderer;

    // Tiles automatically added to and removed from grid over lifetime
    private void OnEnable()
    {
        Grid.AddTile(this);

        if (m_teleportTile)
        {
            MessageBus.AddListener(EMessageType.teleportPlayer, TeleportPlayerHere);
        }
    }
    private void OnDisable()
    {
        Grid.RemoveTile(this);

        if (m_teleportTile)
        {
            MessageBus.RemoveListener(EMessageType.teleportPlayer, TeleportPlayerHere);
        }
    }

    private void Awake()
    {
        m_globalSettings = Resources.Load<GlobalTileSettings>("ScriptableObjects/GlobalTileSettings");
        m_collider = GetComponentInChildren<Collider>();
        m_renderer = GetComponentInChildren<MeshRenderer>();
        if (m_renderer)
        {
            m_normalMaterial = m_renderer.material;
            m_highlightedMaterial = new Material(m_highlightedMaterial);

            m_highlightedMaterial.SetTexture("_MainTex", m_normalMaterial.mainTexture);
        }
    }

    // Moves the player to this tile - called when teleporter is used
    private void TeleportPlayerHere(string _null)
    {
        FindObjectOfType<Player>().transform.position = transform.position;
        Debug.Log("Teleporting player");
    }

    // Returns null if chunk failed to raise
    public Chunk TryRaiseChunk()
    {
        Chunk newChunk = null;

        // Cannot raise a type of none
        if (m_chunkType == EChunkType.none || m_chunkType == EChunkType.lava || IsOccupied())
        {
            return newChunk;
        }

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

    // Used in tutorial - be careful with it - raises a chunk regardless of anything
    public void ForceRaiseChunk()
    {
        // Spawn new chunk
        GameObject chunkPrefab = m_globalSettings.m_chunkPrefabs[(int)m_chunkType];
        Chunk newChunk = Instantiate(chunkPrefab, transform.position, Quaternion.identity, null).GetComponent<Chunk>();
        newChunk.m_chunkType = m_chunkType;
        newChunk.RaiseChunk();

        MessageBus.TriggerEvent(EMessageType.chunkRaise);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.small);
        EffectsManager.SpawnEffect(EffectsManager.EEffectType.rockSummon, newChunk.transform.position, Quaternion.identity, newChunk.transform.localScale, 2.0f);
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
    public EChunkType GetTileType()
    {
        return m_chunkType;
    }

    public void SetChunkType(EChunkType _type)
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

    public void SetHighlighted(bool _highlighted)
    {
        if (_highlighted)
        {
            m_renderer.material = m_highlightedMaterial;
        }
        else
        {
            m_renderer.material = m_normalMaterial;
        }
    }
}
