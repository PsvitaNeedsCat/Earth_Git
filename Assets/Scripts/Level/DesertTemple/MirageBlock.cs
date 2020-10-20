using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageBlock : MirageParent
{
    public bool m_damagesPlayer = false;

    private bool m_isPlayerInside = false;
    private bool m_attemptToSolidify = false;

    protected override void OnDisable()
    {
        base.OnDisable();

        m_isPlayerInside = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if player has entered the block
        if (other.GetComponent<Player>())
        {
            m_isPlayerInside = true;
        }

        Player player = other.gameObject.GetComponent<Player>();

        if (m_damagesPlayer && player && !other.isTrigger && m_playerEffect != m_effect)
        {
            player.GetComponent<HealthComponent>().Health -= 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if player has left the block
        if (other.GetComponent<Player>())
        {
            m_isPlayerInside = false;
        }
    }

    private void Update()
    {
        // If attempting to solidify - wait for player to be outside the block
        if (m_attemptToSolidify && !m_isPlayerInside)
        {
            m_attemptToSolidify = false;
            CanWalkThrough(false);

            if (m_damagesPlayer)
            {
                m_collider.isTrigger = true;
            }
        }
    }

    public override void PowerChanged(string _powerName)
    {
        // Convert to enum
        m_playerEffect = StringToEffect(_powerName);

        // Try to solidify
        m_attemptToSolidify = m_playerEffect != m_effect;
        if (!m_attemptToSolidify)
        {
            CanWalkThrough(true);
        }

        if (m_damagesPlayer)
        {
            m_collider.isTrigger = true;
        }

        if (!m_attemptToSolidify)
        {
            return;
        }

        // Check if chunk is inside
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.45f, 0.45f, 0.45f));
        foreach (Collider collider in colliders)
        {
            // If hit a chunk
            Chunk chunk = collider.GetComponentInParent<Chunk>();
            if (chunk)
            {
                chunk.OnDeath();
                continue;
            }
        }
    }
}
