using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class SpittingEnemy : MonoBehaviour
{
    // Public variables


    // Private variables
    private float m_attackTimer = 0.0f;
    [SerializeField] private Transform m_projectileSpawn;
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
        if (other.GetComponent<Chunk>())
        {
            // Destroy chunk
            Destroy(other.gameObject);
            // Destroy itself
            Destroy(this.gameObject);
        }
    }

    private void FireProjectile()
    {
        MessageBus.TriggerEvent(EMessageType.enemySpit);

        m_attackTimer = m_settings.m_spitCooldown;

        // Instantiate projectile
        Projectile projectile = Instantiate(m_spitProjectile, m_projectileSpawn.position, m_projectileSpawn.rotation).GetComponent<Projectile>();
        projectile.Init(m_settings.m_spitDamage);
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * m_settings.m_spitProjectileSpeed;

        // Tween scale
        Vector3 projScale = projectile.transform.localScale;
        projectile.transform.localScale = projScale * 0.1f;
        projectile.transform.DOScale(projScale, 0.5f).SetEase(Ease.OutElastic);
        projectile.transform.parent = RoomManager.Instance.GetActiveRoom().transform;
    }
}
