using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private GlobalTileSettings m_settings;
    private Collider m_lavaTrigger;
    [SerializeField] private Collider m_lavaCollider;
    [SerializeField] private MeshRenderer m_meshRenderer;
    [SerializeField] private Material m_stoneMat;
    [SerializeField] private ParticleSystem m_particles;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalTileSettings>("ScriptableObjects/GlobalTileSettings");
        Debug.Assert(m_settings, "GlobalTileSettings could not be found");

        m_lavaTrigger = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            // Push player back
            Vector3 dir = (player.transform.position - transform.position);
            dir.y = 0.0f;
            dir.Normalize();
            player.KnockBack(dir);
            player.GetComponent<HealthComponent>().Health -= 1;

            return;
        }

        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk)
        {
            if (chunk.m_currentEffect == eChunkEffect.water)
            {
                // Turn lava to stone
                TurnToStone();
            }
            else
            {
                // Block cannot handle the heat
                chunk.GetComponent<HealthComponent>().Health = 0;
            }

            return;
        }
    }

    private void TurnToStone()
    {
        MessageBus.TriggerEvent(EMessageType.lavaToStone);

        m_lavaTrigger.enabled = false;
        m_lavaCollider.enabled = false;
        m_meshRenderer.material = m_stoneMat;
        m_particles.Stop();
    }
}
