using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private GlobalTileSettings m_settings;
    private Collider m_lavaCollider;

    // Temp
    [SerializeField] private MeshRenderer m_meshRenderer;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalTileSettings>("ScriptableObjects/GlobalTileSettings");
        Debug.Assert(m_settings, "GlobalTileSettings could not be found");

        m_lavaCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            // Push player back
            Vector3 dir = player.transform.position - transform.position;

            player.GetComponent<Rigidbody>().AddForce(dir * m_settings.m_lavaPushForce, ForceMode.Impulse);

            player.GetComponent<HealthComponent>().Health -= 1;

            return;
        }

        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk)
        {
            if (chunk.m_currentEffect == eChunkEffect.waterTrail)
            {
                // Turn lava to stone
                TurnToStone();
            }
            else
            {
                // Block cannot handle the heat
                Destroy(chunk.gameObject);
            }

            return;
        }
    }

    private void TurnToStone()
    {
        m_lavaCollider.enabled = false;
        m_meshRenderer.material.color = Color.grey;
    }
}
