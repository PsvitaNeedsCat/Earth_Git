﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    // Public variables
    [HideInInspector] public EChunkEffect m_effect = EChunkEffect.none;

    // Private variables
    private GlobalPlayerSettings m_settings;
    private Vector3 m_playerPos;
    private List<Chunk> m_collidedChunks = new List<Chunk>();

    // Called when hurtbox is instantiated
    public void Init(Vector3 _pos, EChunkEffect _effect)
    {
        m_playerPos = _pos;
        m_effect = _effect;

        StartCoroutine(DestroyAfter());
    }

    private void Awake()
    {
        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");

        // Increase size if using fire punch
        if (Player.s_currentEffect == EChunkEffect.fire)
        {
            Vector3 newScale = transform.localScale;
            newScale.z = m_settings.m_firePunchSize;
            transform.localScale = newScale;
        }
    }

    private void Start()
    {
        CheckForCollision();
    }

    // Raycasts for chunks and interacts with them
    private void CheckForCollision()
    {
        Vector3 halfExtents = transform.localScale * 0.5f;
        Collider[] colliders = Physics.OverlapBox(transform.position, halfExtents, transform.rotation);

        Chunk closestChunk = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < colliders.Length; i++)
        {
            Chunk chunk = colliders[i].GetComponentInParent<Chunk>();
            if (chunk)
            {
                float distance = (chunk.transform.position - transform.position).magnitude;
                if (distance < closestDist)
                {
                    closestDist = distance;
                    closestChunk = chunk;
                }
            }
        }

        PunchChunk(closestChunk);
    }

    // Punches a given chunk
    private void PunchChunk(Chunk _chunk)
    {
        if (!_chunk)
        {
            return;
        }

        Vector3 hitDir = _chunk.transform.position - m_playerPos;
        hitDir.y = 0.0f;
        hitDir.Normalize();

        Vector3 cardinal = hitDir.Cardinal();
        _chunk.Hit(cardinal * m_settings.m_chunkHitForce, m_effect);

        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.small);
    }

    private IEnumerator DestroyAfter()
    {
        int frameCounter = 0;
        while (frameCounter < m_settings.m_hurtboxFramesToSkip)
        {
            ++frameCounter;
            yield return null;
        }

        Destroy(gameObject);
    }
}
