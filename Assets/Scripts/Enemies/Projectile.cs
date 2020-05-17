using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int m_damage;

    public void Init(int _damage) => m_damage = _damage;

    private void OnTriggerEnter(Collider other)
    {
        HealthComponent playerHealth = other.GetComponent<HealthComponent>();

        if (playerHealth)
        {
            playerHealth.Health -= m_damage;
        }

        MessageBus.TriggerEvent(EMessageType.projectileSplash);
        Destroy(this.gameObject);
    }
}
