﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class MoveableBlock : MonoBehaviour
{
    [SerializeField] private Transform m_move_location;
    private Vector3 m_startLocation;
    private GlobalChunkSettings m_chunkSettings;
    private GlobalPlayerSettings m_playerSettings;
    private MeshRenderer m_renderer;

    private void Awake()
    {
        m_startLocation = transform.position;
        m_chunkSettings = Resources.Load<GlobalChunkSettings>("ScriptableObjects/GlobalChunkSettings");
        m_playerSettings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
        m_renderer = GetComponent<MeshRenderer>();
    }

    // Tween to start location
    public void GoToStart()
    {
        Moved(m_startLocation);

        transform.DOKill();
        transform.DOMove(m_startLocation, 0.5f).SetEase(Ease.OutBounce);

        SetMaterial(false);
    }

    // Goes to the other location
    public void GoToEnd()
    {
        Moved(m_move_location.position);

        transform.DOKill();
        transform.DOMove(m_move_location.position, 0.5f).SetEase(Ease.OutBounce);
        SetMaterial(true);
    }

    private void SetMaterial(bool _on)
    {
        // StopAllCoroutines();
        float endValue = (_on) ? 1.0f : 0.0f;

        // StartCoroutine(BossHelper.ChangeMaterialFloatPropertyOver(m_renderer.material, "_TextureBlend", endValue, 0.5f));

        DOTween.Kill(this);

        DOTween.To(() => m_renderer.material.GetFloat("_TextureBlend"), x => m_renderer.material.SetFloat("_TextureBlend", x), endValue, 0.5f).SetEase(Ease.OutSine);
    }

    // Called when the block is moved
    private void Moved(Vector3 _centre)
    {
        if (!m_chunkSettings)
        {
            m_chunkSettings = Resources.Load<GlobalChunkSettings>("ScriptableObjects/GlobalChunkSettings");
        }

        // Check if there is a chunk in the tile space
        float halfExtents = m_chunkSettings.m_chunkHeight * 0.4f;
        Collider[] hits = Physics.OverlapBox(_centre, new Vector3(halfExtents, halfExtents, halfExtents));

        // If raycast hit something
        if (hits.Length > 0)
        {
            // Check each object it hit
            for (int i = 0; i < hits.Length; i++)
            {
                // Check parent is a chunk
                Chunk chunk = hits[i].GetComponentInParent<Chunk>();
                if (!chunk) 
                {
                    continue; 
                }

                // Destroy the chunk
                chunk.OnDeath();

                return;
            }
        }
    }
}
