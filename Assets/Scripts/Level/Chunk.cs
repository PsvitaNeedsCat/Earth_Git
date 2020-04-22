using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public enum eChunkEffect
{
    none,
    waterTrail
}

[RequireComponent(typeof(Rigidbody))]
public class Chunk : MonoBehaviour
{
    // Public variables
    [HideInInspector] public eChunkEffect m_currentEffect = eChunkEffect.none;
    [HideInInspector] public bool m_quietDestroy = false;

    // Private variables
    private Rigidbody m_rigidBody;
    private Vector3 m_spawnPos;
    [SerializeField] private GlobalChunkSettings m_globalSettings;
    [SerializeField] private ChunkSettings m_Settings;
    private bool m_isRaised = false;
    private bool m_silentDestroy = false;

    // Hitboxes
    [SerializeField] private Collider m_posXCollider;
    [SerializeField] private Collider m_negXCollider;
    [SerializeField] private Collider m_posZCollider;
    [SerializeField] private Collider m_negZCollider;
    [SerializeField] private Collider m_mainCollider;

    // Health
    private int m_curHealth;
    public int Health
    {
        get { return m_curHealth; }
        set
        {
            m_curHealth = value;
            if (Health <= 0)
            {
                Death();
            }
        }
    }

    private void Awake()
    {
        // Set values
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.isKinematic = true;
        m_curHealth = m_Settings.m_maxHealth;
        m_spawnPos = transform.position;

        // Call event handler
    }

    private void OnApplicationQuit()
    {
        m_globalSettings.m_isQuitting = true;
    }

    private void OnDestroy()
    {
        if (m_globalSettings.m_isQuitting) { return; }

        transform.DOKill();
        // Remove chunk

        // Create particles

        if (!m_silentDestroy)
        {
            // Play audio clip
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If colliding with KillBox, ignore

        // Did not hit ground or player
        if (collision.collider.tag != "Ground" && collision.collider.tag != "Player")
        {
            if (IsAgainstWall(m_rigidBody.velocity.normalized))
            {
                SnapChunk();
            }
        }

        // Boss stuff
    }

    // Called when chunk is to be raised
    public void RaiseChunk()
    {
        StartCoroutine(Raise());
    }

    // Raises the chunk
    private IEnumerator Raise()
    {
        // Spawn particles
        // Move particles upwards

        transform.DOKill();
        transform.DOMoveY(m_spawnPos.y + m_globalSettings.m_raiseAmount, m_globalSettings.m_raiseTime).SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(m_globalSettings.m_raiseTime);

        m_isRaised = true;
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }

    // Pushes the chunk in a specific direction when hit by the player
    public void Hit(Vector3 _hitVec)
    {
        if (!m_isRaised) { return; }

        if (IsAgainstWall(_hitVec))
        {
            // Play sound
            Health -= 1;
            return;
        }

        Detach();

        // Enable the correct directional collider
        Vector3 cardinal = _hitVec.normalized;
        if (cardinal.x >= 1.0f) { m_posXCollider.enabled = true; }
        else if (cardinal.x <= -1.0f) { m_negXCollider.enabled = true; }
        else if (cardinal.z >= 1.0f) { m_posZCollider.enabled = true; }
        else { m_negZCollider.enabled = true; } // else if (cardinal.z <= -1.0f)

        m_rigidBody.AddForce(_hitVec, ForceMode.Impulse);

        // Play sound
    }

    // Checks if the chunk is against a wall using a raycast
    private bool IsAgainstWall(Vector3 _hitVec)
    {
        // Start position - almost the bottom of the chunk
        Vector3 checkPosition = transform.position;
        checkPosition.y -= m_globalSettings.m_chunkHeight * 0.5f - 0.5f;

        // Raycast in the direction of the hit vector for half a chunk's length
        RaycastHit hit;
        if (Physics.Raycast(checkPosition, _hitVec, out hit, m_globalSettings.m_wallCheckDistance, m_globalSettings.m_wallLayers))
        {
            // If collided with itself
            if (hit.collider.gameObject.name == this.gameObject.name) { return false; }

            // Hit something
            return true;
        }

        return false;
    }

    // Makes the chunk moveable
    public void Detach()
    {
        m_rigidBody.isKinematic = false;
        m_mainCollider.enabled = false;
    }

    private void SnapChunk()
    {
        // Play sound

        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.isKinematic = true;

        // Change colliders
        m_posXCollider.enabled = false;
        m_negXCollider.enabled = false;
        m_posZCollider.enabled = false;
        m_negZCollider.enabled = false;
        m_mainCollider.enabled = true;

        // Find nearest grid tile

        // Snap to the nearest grid tile
    }
}
