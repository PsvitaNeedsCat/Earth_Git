﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int m_damage;

    public void Init(int _damage) => m_damage = _damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Lava") { return; }

        Player playerHealth = other.GetComponent<Player>();

        if (playerHealth)
        {
            playerHealth.GetComponent<HealthComponent>().Health -= m_damage;
        }

        MessageBus.TriggerEvent(EMessageType.projectileSplash);
        Destroy(this.gameObject);
    }
}
