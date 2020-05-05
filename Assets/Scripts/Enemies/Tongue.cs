﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Tongue : MonoBehaviour
{
    private GlobalEnemySettings m_settings;

    private Chunk m_attachedChunk = null;
    private TongueEnemy m_parent;
    [SerializeField] private Transform m_tongueMaxPosition;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        m_parent = transform.parent.GetComponent<TongueEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponent<Chunk>();

        // If tongue hit a chunk
        if (chunk)
        {
            chunk.SnapToTongue(transform.position);
            chunk.transform.parent = transform;
            m_attachedChunk = chunk;
        }
        else
        {
            Player player = other.GetComponent<Player>();

            if (player)
            {
                // Hit player object
                player.GetComponent<HealthComponent>().Health -= m_settings.m_tongueDamage;
            }
        }
    }

    // Extends the tongue
    public void Extend()
    {
        // Tween to position
        transform.DOKill(false);
        transform.DOMove(m_tongueMaxPosition.position, 1.0f).OnComplete(() => Retract());
    }

    // Retarcts the tongue
    private void Retract()
    {
        m_parent.m_state = TongueEnemy.State.retracting;

        transform.DOKill(false);
        transform.DOMove(m_parent.transform.position, 1.0f).OnComplete(() => m_parent.Swallow());
    }

    // Is called when tongue enemy swallows
    public eChunkType GetAttached()
    {
        if (!m_attachedChunk) { return eChunkType.none; }

        eChunkType type = m_attachedChunk.m_chunkType;

        Destroy(m_attachedChunk.gameObject);

        return type;
    }
}
