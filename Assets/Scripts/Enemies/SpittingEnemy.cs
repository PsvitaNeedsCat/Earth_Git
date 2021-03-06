﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class SpittingEnemy : MonoBehaviour
{
    // Public variables


    // Private variables
    private float m_attackTimer = 0.0f;
    private GlobalEnemySettings m_settings;
    [SerializeField] private GameObject m_spitProjectile;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        m_attackTimer = m_settings.m_spitCooldown;
    }

    private void Update()
    {
        m_attackTimer -= Time.deltaTime;

        if (m_attackTimer <= 0.0f)
        {
            FireProjectile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk && chunk.m_currentEffect == EChunkEffect.none)
        {
            // Destroy chunk
            other.GetComponent<HealthComponent>().Health = 0;

            Dead();
        }
    }

    // Kills the spitting enemy - called when chunk collides with statue
    private void Dead()
    {
        Destroy(gameObject);

        EffectsManager.SpawnEffect(EffectsManager.EEffectType.statueBreak, transform.position, Quaternion.identity, Vector3.one, 1.0f);
    }

    private void FireProjectile()
    {
        MessageBus.TriggerEvent(EMessageType.enemySpit);

        m_attackTimer = m_settings.m_spitCooldown;

        // Instantiate projectile
        Vector3 spawnPos = transform.position + (transform.forward * m_settings.m_spitSpawnDist);
        Projectile projectile = Instantiate(m_spitProjectile, spawnPos, transform.rotation).GetComponent<Projectile>();
        projectile.Init(m_settings.m_spitDamage);
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * m_settings.m_spitProjectileSpeed;

        // Tween scale
        Vector3 projScale = projectile.transform.localScale;
        projectile.transform.localScale = projScale * 0.1f;
        projectile.transform.DOScale(projScale, 0.5f).SetEase(Ease.OutElastic);
        projectile.transform.parent = RoomManager.Instance.GetActiveRoom().transform;
    }
}
