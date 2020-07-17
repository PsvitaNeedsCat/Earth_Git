using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private eChunkEffect m_effectType = eChunkEffect.none;

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

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player)
        {
            player.GetComponent<HealthComponent>().Health -= 1;
        }
    }

    // Spawns a bullet
    private void FireBullet()
    {
        Vector3 spawnPos = transform.position + (transform.forward * m_settings.m_snakeBulletSpawnDist);
        MirageBullet bullet = Instantiate(m_bulletRef, spawnPos, transform.rotation).GetComponent<MirageBullet>();
        bullet.Init(m_effectType, m_playerRef.GetCurrentPower());
        bullet.transform.parent = transform;
    }
}
