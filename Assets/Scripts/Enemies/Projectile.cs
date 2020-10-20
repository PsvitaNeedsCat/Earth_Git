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

            if (m_destroyedSignal == EMessageType.fieryExplosion)
            {
                MessageBus.TriggerEvent(EMessageType.fireProjectileHitPlayer);
            }
        }

        OnDeath();
    }

    private void OnDeath()
    {
        MessageBus.TriggerEvent(m_destroyedSignal);
        if (m_destroyedSignal == EMessageType.sandProjectileDestroyed)
        {
            EffectsManager.EEffectType effectType = EffectsManager.EEffectType.sandProjectileDestroyed;
            EffectsManager.SpawnEffect(effectType, transform.position, transform.rotation);
        }
        else if (m_destroyedSignal == EMessageType.projectileSplash)
        {
            EffectsManager.EEffectType effectType = EffectsManager.EEffectType.waterProjectileDestroyed;
            Vector3 scale = Vector3.one * 0.5f;
            EffectsManager.SpawnEffect(effectType, transform.position, transform.rotation, scale);
        }
        else if (m_destroyedSignal == EMessageType.fieryExplosion)
        {
            EffectsManager.EEffectType effectType = EffectsManager.EEffectType.fieryExplosion;
            Quaternion rotation = Quaternion.LookRotation(-transform.forward);
            Vector3 scale = Vector3.one * 0.5f;
            EffectsManager.SpawnEffect(effectType, transform.position, rotation, scale, 0.5f);
        }
        Destroy(gameObject);
    }
}
