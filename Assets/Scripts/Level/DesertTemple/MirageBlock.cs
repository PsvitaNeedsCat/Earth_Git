using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageBlock : MirageParent
{
    private bool m_isPlayerInside = false;
    private bool m_attemptToSolidify = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if player has entered the block
        if (other.GetComponent<Player>())
        {
            m_isPlayerInside = true;
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
            CanWalkThrough(true);
        }
    }

    public override void PowerChanged(string _powerName)
    {
        // Convert to enum
        m_currentEffect = StringToEffect(_powerName);

        // Try to solidify
        m_attemptToSolidify = m_currentEffect != m_effectType;
        if (!m_attemptToSolidify)
        {
            CanWalkThrough(true);
        }

        // Check if chunk is inside
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(0.45f, 0.45f, 0.45f), Vector3.one, Quaternion.identity, 0.0f);
        foreach (RaycastHit hit in hits)
        {
            // If hit a chunk
            if (hit.collider.GetComponentInParent<Chunk>())
            {
                if (!hit.collider.isTrigger)
                {
                    Destroy(hit.collider.transform.parent.gameObject);
                    continue;
                }
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
