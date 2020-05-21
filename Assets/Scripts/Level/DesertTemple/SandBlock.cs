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

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_chunkSettings = Resources.Load<GlobalChunkSettings>("ScriptableObjects/GlobalChunkSettings");
    }

    private void OnEnable()
    {
        SandManager manager = FindObjectOfType<SandManager>();
        if (manager)
        {
            manager.m_sandBlocks.Add(this.gameObject);
        }
        else { Debug.Log("Could not find Sand Manager"); }
    }
    private void OnDisable()
    {
        SandManager manager = FindObjectOfType<SandManager>();
        if (manager)
        {
            manager.m_sandBlocks.Remove(this.gameObject);
        }
        else { Debug.Log("Could not find Sand Manager"); }
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

        // Block is falling and hits the ground
        if (m_isFalling && other.tag == "Ground") { StopFalling(); }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player) { player.m_inSand = false; }
    }

    // Called when glass is to break
    private void Break()
    {
        // Call message bus

        Destroy(this.gameObject);
    }

    // Called when sand is hit by fire
    private void TurnToGlass()
    {
        // Call message bus

        m_isGlass = true;
        GetComponent<Collider>().isTrigger = false;
        GetComponent<MeshRenderer>().material = m_glassMat;
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
