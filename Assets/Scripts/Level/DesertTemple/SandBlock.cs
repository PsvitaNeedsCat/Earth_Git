using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBlock : MonoBehaviour
{
    [HideInInspector] public bool m_isGlass = false;

    [SerializeField] private Material m_glassMat;
    private Rigidbody m_rigidbody;
    private bool m_isFalling = false;
    private GlobalChunkSettings m_chunkSettings;
    private Vector3 m_fallingBounds = new Vector3(0.8f, 1.0f, 0.8f);
    private GameObject m_chunkInside = null;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_chunkSettings = Resources.Load<GlobalChunkSettings>("ScriptableObjects/GlobalChunkSettings");
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player) { player.m_inSand = true; }

        // If player punched block
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox)
        {
            if (m_isGlass) { Break(); } // Break if glass
            else if (hurtbox.m_effect == eChunkEffect.fire) { TurnToGlass(); } // Turn to glass if currently sand and has fire equipped
        }

        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk && !m_chunkInside) { m_chunkInside = other.gameObject; }

        // Block is falling and hits the ground
        if (m_isFalling && other.tag == "Ground") { StopFalling(); }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player) { player.m_inSand = false; }

        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk && m_chunkInside == chunk) { m_chunkInside = null; }
    }

    private void OnDestroy()
    {
        MessageBus.TriggerEvent(EMessageType.glassDestroyed);
    }

    // Called when glass is to break
    private void Break()
    {
        Destroy(this.gameObject);
    }

    // Called when sand is hit by fire
    private void TurnToGlass()
    {
        MessageBus.TriggerEvent(EMessageType.lavaToStone);

        m_isGlass = true;
        GetComponent<Collider>().isTrigger = false;
        GetComponent<MeshRenderer>().material = m_glassMat;

        // Break chunk inside
        if (m_chunkInside) { Destroy(m_chunkInside); }
    }

    // Makes the sand block fall - called from external source
    public void Fall()
    {
        // Physics
        m_isFalling = true;
        m_rigidbody.isKinematic = false;
        m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        m_rigidbody.useGravity = true;

        // Collider
        GetComponent<BoxCollider>().size = m_fallingBounds;
    }

    // Called when sand hits the ground
    private void StopFalling()
    {
        // Physics
        m_isFalling = false;
        m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        m_rigidbody.isKinematic = true;
        m_rigidbody.useGravity = false;

        // Collider
        GetComponent<BoxCollider>().size = Vector3.one;

        // Snap to tile
        Tile tile = Grid.FindClosestTileAny(transform.position);
        if (tile)
        {
            Vector3 newPos = tile.transform.position;
            newPos.y += m_chunkSettings.m_raiseAmount;
            transform.position = newPos;
        }
    }
}
