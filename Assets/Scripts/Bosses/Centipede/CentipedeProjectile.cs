using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeProjectile : MonoBehaviour
{
    private void Awake()
    {
        Destroy(this.gameObject, CentipedeBoss.m_settings.m_projectileLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        // Debug.Log("Collided with " + other.name);

        HealthComponent healthComp = other.GetComponent<HealthComponent>();

        if (healthComp?.m_type == HealthComponent.EHealthType.player)
        {
            healthComp.Health -= 1;
        }

        Destroy(this.gameObject);
    }
}
