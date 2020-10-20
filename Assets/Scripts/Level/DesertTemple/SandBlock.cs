using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBlock : MonoBehaviour
{
    [HideInInspector] public bool m_isGlass = false;
    [HideInInspector] public bool m_isDestroyed = false;

    [SerializeField] private Material m_glassMat;
    [SerializeField] private bool m_glassOverride = false;
    private Rigidbody m_rigidbody;
    private bool m_isFalling = false;
    private GlobalChunkSettings m_chunkSettings;
    private GameObject m_chunkInside = null;
    private bool m_isPlayerInside = false;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_chunkSettings = Resources.Load<GlobalChunkSettings>("ScriptableObjects/GlobalChunkSettings");

        if (m_glassOverride)
        {
            TurnToGlass(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            m_isPlayerInside = true;
            player.m_inSand = true; 
        }

        // If player punched block
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox)
        {
            if (m_isGlass)
            {
                Break();
            } // Break if glass
            else if (hurtbox.m_effect == EChunkEffect.fire)
            {
                TurnToGlass();
            } // Turn to glass if currently sand and has fire equipped
        }

        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk)
        {
            if (chunk.m_currentEffect == EChunkEffect.fire)
            {
                chunk.OnDeath();
                TurnToGlass();
            }

            if (!m_chunkInside)
            {
                m_chunkInside = chunk.gameObject;
            }
        }

        // Block is falling and hits the ground
        if (m_isFalling && other.tag == "Ground")
        {
            StopFalling(); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            m_isPlayerInside = false;
            player.m_inSand = false; 
        }

        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk)
        {
            if (m_chunkInside.GetInstanceID() == chunk.gameObject.GetInstanceID())
            {
                m_chunkInside = null;
            }
        }
    }

    private void OnDestroy()
    {
        m_isDestroyed = true;

        MessageBus.TriggerEvent(EMessageType.glassDestroyed);

        if (m_isPlayerInside && !m_isFalling)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player)
            {
                player.m_inSand = false;
            }
        }
    }

    // Destroys the object
    private void Break()
    {
        MessageBus.TriggerEvent(EMessageType.glassDestroyed);
        EffectsManager.SpawnEffect(EffectsManager.EEffectType.glassBreak, transform.position, Quaternion.identity, Vector3.one, 1.0f);

        Destroy(gameObject);
    }

    // Makes the sand block glass (changes it to transparent white, makes the collider solid
    public void TurnToGlass(bool _silent = false)
    {
        if (!_silent)
        {
            MessageBus.TriggerEvent(EMessageType.lavaToStone);
        }

        m_isGlass = true;
        GetComponent<Collider>().isTrigger = false;
        GetComponentInChildren<MeshRenderer>().material = m_glassMat;

        transform.Find("Mesh").localScale = new Vector3(1.01f, 1.0f, 1.01f);
        transform.Find("Mesh").localPosition = Vector3.zero;

        // Break chunk inside
        if (m_chunkInside)
        {
            Destroy(m_chunkInside);
        }
    }

    // Changes some rigidbody settings so that the sand will begin to fall with gravity
    public void Fall()
    {
        // Physics
        m_isFalling = true;
        m_rigidbody.isKinematic = false;
        m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        m_rigidbody.useGravity = true;
    }

    // Changes some rigidbody settings so that the sand won't move by gravity or other means
    private void StopFalling()
    {
        // Physics
        m_isFalling = false;
        m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        m_rigidbody.isKinematic = true;
        m_rigidbody.useGravity = false;

        // Snap to tile
        Tile tile = Grid.FindClosestTileAny(transform.position);
        if (tile)
        {
            Vector3 newPos = tile.transform.position;
            newPos.y += m_chunkSettings.m_raiseAmount;
            transform.position = newPos;
        }

        MessageBus.TriggerEvent(EMessageType.sandLand);
    }

    // Returns if the sand is falling or not. It is grounded if it is not falling
    public bool IsGrounded()
    {
        return !m_isFalling;
    }
}
