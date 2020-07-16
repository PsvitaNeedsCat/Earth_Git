using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraPot : MonoBehaviour
{
    public Transform m_projectileParent;
    public float m_projectileSpawnDistance;
    private GameObject m_projectilePrefab;

    private void Awake()
    {
        m_projectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/CobraPotProjectile");
    }

    public void FireProjectile()
    {
        Vector3 spawnPosition = transform.position + transform.forward * m_projectileSpawnDistance;
        GameObject projectile = Instantiate(m_projectilePrefab, spawnPosition, transform.rotation, m_projectileParent);
        Destroy(projectile, CobraHealth.StateSettings.m_potProjectileLifetime);
        projectile.GetComponent<Rigidbody>().velocity = transform.forward * CobraHealth.StateSettings.m_potProjectileSpeed;

        transform.DOPunchScale(Vector3.one * 0.2f, 0.15f).SetEase(Ease.InOutElastic);
        transform.DOPunchPosition(Vector3.up * 0.2f, 0.2f).SetEase(Ease.InCubic);
    }
}
