using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    // Public variables


    // Private variables
    private int m_framesSkipped = 0;
    private GlobalPlayerSettings m_settings;
    private Vector3 m_playerPos;
    private eChunkEffect m_effect = eChunkEffect.none;

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
        Chunk chunk = other.transform.parent.GetComponent<Chunk>();

        // Check collision is a chunk
        if (chunk)
        {
            Vector3 hitDir = chunk.transform.position - m_playerPos;
            hitDir.y = 0.0f;
            hitDir.Normalize();

            Vector3 cardinal = GetCardinalDir(hitDir);
            if (chunk.Hit(cardinal * m_settings.m_chunkHitForce))
            {
                chunk.m_currentEffect = m_effect;
            }

            Destroy(this.gameObject);
        }
    }

    // Get the closest cardinal direction of a given vector
    private Vector3 GetCardinalDir(Vector3 _dir)
    {
        Vector3 cardinalDir;

        if (Mathf.Abs(_dir.z) > Mathf.Abs(_dir.x))
        {
            if (_dir.z > 0.0f) { cardinalDir = Vector3.forward; }
            else { cardinalDir = Vector3.back; }
        }
        else
        {
            if (_dir.x > 0.0f) { cardinalDir = Vector3.right; }
            else { cardinalDir = Vector3.left; }
        }

        return cardinalDir;
    }
}
