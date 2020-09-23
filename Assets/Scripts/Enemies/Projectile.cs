﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public EMessageType m_destroyedSignal = EMessageType.projectileSplash;

    public int m_damage = 1;

    public void Init(int _damage)
    {
        m_damage = _damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        SandBlock sand = other.GetComponent<SandBlock>();

        if (sand && !sand.m_isGlass)
        {
            return;
        }

        if (other.tag == "Lava")
        {
            return;
        }

        Player playerHealth = other.GetComponent<Player>();

        if (playerHealth)
        {
            playerHealth.GetComponent<HealthComponent>().Health -= m_damage;
        }

        MessageBus.TriggerEvent(m_destroyedSignal);
        Destroy(gameObject);
    }
}
