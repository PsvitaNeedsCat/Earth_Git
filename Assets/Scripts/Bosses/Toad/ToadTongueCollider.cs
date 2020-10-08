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
        // Don't stick to a new chunk or deal damage if there is a chunk attached
        if (!m_attachedChunk)
        {
            HealthComponent healthComp = _other.GetComponent<HealthComponent>();
            // If the player is hit, damage them and retract the tongue
            if (healthComp && _other.GetComponent<PlayerController>())
            {
                healthComp.Health -= m_damage;
                m_tongueAttack.RetractTongue();
                return;
            }

            // Check if hit chunk
            Chunk chunk = _other.GetComponentInParent<Chunk>();
            if (chunk)
            {
                MessageBus.TriggerEvent(EMessageType.tongueStuck);

                chunk.Detach();
                chunk.OnStuckToTongue();
                chunk.transform.parent = transform;

                m_attachedChunk = chunk;
                m_tongueAttack.RetractTongue();

                return;
            }
        }
    }

    public void OnRetracted()
    {
        m_tongueAttack.OnRetracted();
    }

    public EChunkType Swallow()
    {
        if (!m_attachedChunk) { return EChunkType.none; }

        EChunkType type = m_attachedChunk.m_chunkType;

        Destroy(m_attachedChunk.gameObject);

        return type;
    }
}