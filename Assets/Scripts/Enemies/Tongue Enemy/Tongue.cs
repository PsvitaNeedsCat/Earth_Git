using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Tongue : MonoBehaviour
{
    private GlobalEnemySettings m_settings;

    private Chunk m_attachedChunk = null;
    private TongueEnemy m_parent = null;
    private Animator m_animator = null;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        m_parent = transform.parent.GetComponent<TongueEnemy>();
        m_animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Reset animator
        m_animator.ResetTrigger("Extend");
        m_animator.ResetTrigger("Retract");
        m_animator.SetFloat("ExtendDirection", 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hits a chunk
        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk)
        {
            chunk.SnapToTongue(GetComponent<Collider>().ClosestPoint(chunk.transform.position), m_parent.transform.position);
            m_attachedChunk = chunk;

            MessageBus.TriggerEvent(EMessageType.tongueStuck);

            Retract();
            return;
        }

        // Hits player
        Player player = other.GetComponent<Player>();
        if (player)
        {
            // Hit player object
            player.GetComponent<HealthComponent>().Health -= m_settings.m_tongueDamage;

            Retract();
            return;
        }
    }

    // Extends the tongue
    public void Extend()
    {
        MessageBus.TriggerEvent(EMessageType.enemyTongueExtend);

        // Start animation
        m_animator.SetTrigger("Extend");
    }

    // Called by animator when retract animation begins
    public void Retract()
    {
        m_parent.m_state = TongueEnemy.State.retracting;

        m_animator.SetFloat("ExtendDirection", -1.0f);
    }

    public void Swallow()
    {
        m_animator.SetFloat("ExtendDirection", 1.0f);
    }

    // Is called when tongue enemy swallows
    public EChunkType GetAttached()
    {
        if (!m_attachedChunk) { return EChunkType.none; }

        EChunkType type = m_attachedChunk.m_chunkType;

        Destroy(m_attachedChunk.gameObject);

        return type;
    }
}
