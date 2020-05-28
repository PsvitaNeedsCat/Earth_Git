using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadTongueCollider : MonoBehaviour
{
    public ToadTongueAttack m_tongueAttack;

    Chunk m_attachedChunk = null;
    readonly int m_damage = 1;

    private void OnTriggerEnter(Collider _other)
    {
        Debug.Log("Tongue collided with " + _other.name);

        HealthComponent healthComp = _other.GetComponent<HealthComponent>();   

        // If the player is hit, damage them and retract the tongue
        if (healthComp && _other.GetComponent<PlayerController>())
        {
            healthComp.Health -= m_damage;
            m_tongueAttack.RetractTongue();
            return;
        }

        // If a chunk isn't attached, see if we have hit one
        if (!m_attachedChunk)
        {
            Chunk chunk = _other.GetComponentInParent<Chunk>();

            if (chunk)
            {
                MessageBus.TriggerEvent(EMessageType.tongueStuck);

                chunk.Detach();
                chunk.OnStuckToTongue();
                chunk.transform.parent = this.transform;

                m_attachedChunk = chunk;
                m_tongueAttack.RetractTongue();

                return;
            }
        }
    }

    public void OnRetracted()
    {
        Debug.Log("OnRetracted called");

        m_tongueAttack.OnRetracted();
    }

    public eChunkType Swallow()
    {
        if (!m_attachedChunk) { return eChunkType.none; }

        eChunkType type = m_attachedChunk.m_chunkType;

        Destroy(m_attachedChunk.gameObject);

        return type;
    }
}