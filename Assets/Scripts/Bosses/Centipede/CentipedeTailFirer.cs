using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CentipedeTailFirer : MonoBehaviour
{
    public GameObject m_tailObject;
    public GameObject m_projectilePrefab;
    public float m_spawnDistance;
    public float m_spawnHeight;

    // Fire four projectiles in cardinal directions
    public void FireAll(bool _tailDamaged)
    {
        m_tailObject.transform.DOPunchScale(Vector3.one * 0.2f, 0.1f);
        MessageBus.TriggerEvent(EMessageType.centipedeTailFire);

        Fire(transform.localRotation * Vector3.right, _tailDamaged);
        Fire(transform.localRotation * Vector3.left, _tailDamaged);
        Fire(transform.localRotation * Vector3.forward, _tailDamaged);
        Fire(transform.localRotation * Vector3.back, _tailDamaged);
    }

    // Fire a signle projectile in a direction
    private void Fire(Vector3 _dir, bool _tailDamaged)
    {
        float projectileSpeed = (_tailDamaged) ? CentipedeBoss.m_settings.m_projectileSpeedDamaged : CentipedeBoss.m_settings.m_projectileSpeed;

        GameObject newProjectile = Instantiate(m_projectilePrefab, transform.position + m_spawnDistance * _dir + Vector3.up * m_spawnHeight, Quaternion.identity, null);
        newProjectile.GetComponent<Rigidbody>().velocity = _dir * projectileSpeed;
    }
}
