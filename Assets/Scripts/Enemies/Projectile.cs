using System.Collections;
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

        OnDeath();
    }

    private void OnDeath()
    {
        MessageBus.TriggerEvent(m_destroyedSignal);
        if (m_destroyedSignal == EMessageType.sandProjectileDestroyed)
        {
            EffectsManager.SpawnEffect(EffectsManager.EEffectType.sandProjectileDestroyed, transform.position, transform.rotation);
        }
        else if (m_destroyedSignal == EMessageType.projectileSplash)
        {
            EffectsManager.SpawnEffect(EffectsManager.EEffectType.waterProjectileDestroyed, transform.position, transform.rotation);
        }
        else if (m_destroyedSignal == EMessageType.fieryExplosion)
        {
            Quaternion rotation = Quaternion.LookRotation(-transform.forward);
            Vector3 scale = Vector3.one * 0.5f;
            EffectsManager.SpawnEffect(EffectsManager.EEffectType.fieryExplosion, transform.position, rotation, scale, 0.5f);
        }
        Destroy(gameObject);
    }
}
