using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    // Public variables
    [HideInInspector] public eChunkEffect m_effect = eChunkEffect.none;

    // Private variables
    private int m_framesSkipped = 0;
    private GlobalPlayerSettings m_settings;
    private Vector3 m_playerPos;

    // Called when hurtbox is instantiated
    public void Init(Vector3 _pos, eChunkEffect _effect)
    {
        m_playerPos = _pos;
        m_effect = _effect;
    }

    private void Awake()
    {
        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
    }

    private void Update()
    {
        // Check the amount of frames skipped
        if (m_framesSkipped < m_settings.m_framesBeforeDestroy) { m_framesSkipped++; }
        else { Destroy(this.gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponentInParent<Chunk>();

        // Check collision is a chunk
        if (chunk)
        {
            Vector3 hitDir = chunk.transform.position - m_playerPos;
            hitDir.y = 0.0f;
            hitDir.Normalize();

            Vector3 cardinal = hitDir.Cardinal();
            if (chunk.Hit(cardinal * m_settings.m_chunkHitForce))
            {
                chunk.CurrentEffect = m_effect;
            }

            Destroy(this.gameObject);
        }
    }
}
