﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CentipedeTailFirer : MonoBehaviour
{
    public GameObject m_tailObject;
    public GameObject m_projectilePrefab;
    public Transform m_projectileParent;
    public float m_spawnDistance;
    public float m_spawnHeight;

    // Fire four projectiles in cardinal directions
    public void FireAll(bool _tailDamaged)
    {
        m_tailObject.transform.DOPunchScale(Vector3.one * 0.2f, 0.1f);
        MessageBus.TriggerEvent(EMessageType.centipedeTailFire);

        Fire(transform.rotation * Vector3.right, _tailDamaged);
        Fire(transform.rotation * Vector3.left, _tailDamaged);
        Fire(transform.rotation * Vector3.up, _tailDamaged);
        Fire(transform.rotation * Vector3.down, _tailDamaged);
    }

    // Fire a single projectile in a direction
    private void Fire(Vector3 _dir, bool _tailDamaged)
    {
        float projectileSpeed = (_tailDamaged) ? CentipedeBoss.s_settings.m_projectileSpeedDamaged : CentipedeBoss.s_settings.m_projectileSpeed;

        GameObject newProjectile = Instantiate(m_projectilePrefab, transform.position + m_spawnDistance * _dir + Vector3.up * m_spawnHeight, Quaternion.identity, m_projectileParent);
        newProjectile.transform.LookAt(newProjectile.transform.position + _dir);
        newProjectile.GetComponent<Rigidbody>().velocity = _dir * projectileSpeed;
    }
}
