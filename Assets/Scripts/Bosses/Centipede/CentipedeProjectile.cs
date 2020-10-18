using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeProjectile : MonoBehaviour
{
    private void Awake()
    {
        Destroy(this.gameObject, CentipedeBoss.s_settings.m_projectileLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.isTrigger) return;

        // If the player is hit, damage them
        HealthComponent healthComp = other.GetComponent<HealthComponent>();
        if (healthComp?.m_type == HealthComponent.EHealthType.player)
        {
            healthComp.Health -= 1;
        }

        Quaternion rotation = Quaternion.LookRotation(-transform.forward);
        Vector3 scale = Vector3.one * 0.5f;
        EffectsManager.SpawnEffect(EffectsManager.EEffectType.fieryExplosion, transform.position, rotation, scale, 0.5f);

        Destroy(gameObject);
    }
}
