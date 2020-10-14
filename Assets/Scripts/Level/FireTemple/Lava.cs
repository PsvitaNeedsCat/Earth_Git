using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private GlobalTileSettings m_settings;
    private Collider m_lavaTrigger;
    [SerializeField] private Collider m_lavaCollider;
    [SerializeField] protected MeshRenderer m_meshRenderer;
    [SerializeField] private Material m_stoneMat;
    [SerializeField] private ParticleSystem m_particles;

    protected bool m_tweeningChunk = false;
    protected bool m_damagePlayer = true;

    protected virtual void Awake()
    {
        m_settings = Resources.Load<GlobalTileSettings>("ScriptableObjects/GlobalTileSettings");
        Debug.Assert(m_settings, "GlobalTileSettings could not be found");

        m_lavaTrigger = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk)
        {
            if (chunk.m_currentEffect == EChunkEffect.water)
            {
                // Turn lava to stone
                TurnToStone();
            }
            else if (chunk.m_currentEffect == EChunkEffect.none)
            {
                // Block cannot handle the heat
                PrepareToSink(chunk);
            }

            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player && m_damagePlayer)
        {
            // Push player back
            Vector3 dir = (player.transform.position - transform.position);
            dir.y = 0.0f;
            dir.Normalize();
            player.KnockBack(dir);
            player.GetComponent<HealthComponent>().Health -= 1;

            return;
        }
    }

    // Gets the chunk set up to sink into lava
    private void PrepareToSink(Chunk _chunk)
    {
        m_tweeningChunk = true;

        // Remove components
        GameObject chunkObj = _chunk.gameObject;
        Destroy(_chunk);
        Destroy(chunkObj.GetComponent<Rigidbody>());

        // Tween
        Vector3 tweenPos = chunkObj.transform.position;
        Vector3 diff = transform.position - tweenPos;
        diff.y = 0.0f;
        tweenPos += diff;
        chunkObj.transform.DOMove(tweenPos, 0.25f).OnComplete(() => Sink(chunkObj));
    }

    // Sinks the chunk into lava
    private void Sink(GameObject _chunk)
    {
        // Audio
        MessageBus.TriggerEvent(EMessageType.chunkSinking);

        // Tween
        Vector3 sinkPosition = transform.position;
        sinkPosition.y -= 0.6f;
        _chunk.transform.DOMove(sinkPosition, 1.0f).OnComplete(() => DestroyChunk(_chunk));
    }

    // Called by sink - destroys the chunk
    private void DestroyChunk(GameObject _chunk)
    {
        Destroy(_chunk);

        m_tweeningChunk = false;
    }

    // Plays sound then turns the lava to stone
    protected virtual void TurnToStone()
    {
        MessageBus.TriggerEvent(EMessageType.lavaToStone);

        TurnToStoneSilent();
    }

    // Turns the lava to stone without sound - making it walkable
    public void TurnToStoneSilent()
    {
        m_lavaTrigger.enabled = false;
        m_lavaCollider.enabled = false;
        m_meshRenderer.material = m_stoneMat;
        m_particles.Stop();
    }
}
