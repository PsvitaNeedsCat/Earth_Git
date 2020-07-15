using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLavaTrail : MonoBehaviour
{
    private void Awake()
    {
        // Destroy after lifetime is up
        Destroy(this.gameObject, CentipedeBoss.m_settings.m_lavaLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player walks on the lava, damage them and knock them back
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            Vector3 dir = (player.transform.position - transform.position);
            dir.y = 0.0f;
            player.KnockBack(dir.normalized);
            player.GetComponent<HealthComponent>().Health -= 1;

            return;
        }

        // If a water-infused chunk hits the lava, destroy it, otherwise destroy the chunk
        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk)
        {
            if (chunk.m_currentEffect == eChunkEffect.water)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                chunk.GetComponent<HealthComponent>().Health = 0;
            }
        }
    }
}
