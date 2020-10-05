using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadWaveSegment : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HealthComponent healthComp = other.GetComponent<HealthComponent>();

        if (healthComp && other.GetComponent<PlayerController>())
        {
            healthComp.Health -= 1;
        }

        Disable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        EffectsManager.EEffectType effectType = EffectsManager.EEffectType.waveDestroyed;
        Quaternion rotation = Quaternion.LookRotation(transform.forward);
        EffectsManager.SpawnEffect(effectType, transform.position, rotation, Vector3.one, 3.0f);
    }
}
