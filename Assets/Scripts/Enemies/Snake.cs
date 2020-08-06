﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private EChunkEffect m_effectType = EChunkEffect.none;

    private GameObject m_bulletRef = null;
    private GlobalEnemySettings m_settings;
    private Player m_playerRef = null;
    private float m_delayTimer = 0.0f;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");
        m_bulletRef = Resources.Load<GameObject>("Prefabs/Enemies/MirageBullet");
        m_playerRef = FindObjectOfType<Player>();
    }

    // Calls FireBullet every 0.3 seconds
    // Authors: Callum
    private void Update()
    {
        // Countdown fire delay
        if (m_delayTimer <= 0.0f)
        {
            FireBullet();
            m_delayTimer += m_settings.m_snakeFireDelay;
        }

        m_delayTimer -= Time.deltaTime;
    }

    // Instantiates and fires a mirage bullet forwards
    // Authors: Callum
    private void FireBullet()
    {
        Vector3 spawnPos = transform.position + (transform.forward * m_settings.m_snakeBulletSpawnDist);
        spawnPos.y += 0.5f;
        MirageBullet bullet = Instantiate(m_bulletRef, spawnPos, transform.rotation).GetComponent<MirageBullet>();
        bullet.Init(m_effectType, m_playerRef.GetCurrentPower());
        bullet.transform.parent = transform;
        // Tween mesh
        transform.GetChild(0).transform.DORewind();
        transform.GetChild(0).transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
    }
}
