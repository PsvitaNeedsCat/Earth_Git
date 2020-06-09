﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeTrainAttack : CentipedeBehaviour
{
    [System.Serializable]
    public struct TunnelDef
    {
        // The entrance to the tunnel, closest to the arena
        public Transform m_tunnelStart;
        // The far end of the tunnel, furtherest from the arena
        public Transform m_tunnelEnd;
        // The target point for the centipede when emerging from this tunnel
        public Transform m_tunnelTarget;
        // A point used to get the centipede to move to the next area without cutting the corner
        public Transform m_nextCorner;
    }

    public List<TunnelDef> m_tunnels;

    private int m_currentTunnelIndex = 0;
    private CentipedeHealth m_centipedeHealth;

    private void Awake()
    {
        m_centipedeHealth = GetComponent<CentipedeHealth>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        // StartCoroutine(CompleteAfterSeconds(3.0f));
        StartCoroutine(ExitArena());
    }

    // Exit arena to begin tunnel attacks
    private IEnumerator ExitArena()
    {
        // Make centipede move to tunnel end, via tunnel start
        CentipedeMovement.SetTargets(new List<Transform>{ m_tunnels[0].m_tunnelStart, m_tunnels[0].m_tunnelEnd});
        CentipedeMovement.m_seekingTarget = true;

        while (!CentipedeMovement.m_atTarget)
        {
            yield return null;
        }

        CentipedeMovement.m_useTrainSpeed = true;
        CentipedeBoss.m_dropLava = true;
        StartCoroutine(TunnelAttack());
    }

    private IEnumerator TunnelAttack()
    {
        m_centipedeHealth.ActivateSection(true, CentipedeHealth.ESegmentType.head);
        TunnelDef currentTunnel = m_tunnels[m_currentTunnelIndex];
        CentipedeMovement.SetTargets(new List<Transform> { currentTunnel.m_tunnelEnd, currentTunnel.m_tunnelStart, currentTunnel.m_tunnelTarget, currentTunnel.m_nextCorner });

        // Wait for centipede to finish these movements
        while (!CentipedeMovement.m_atTarget)
        {
            yield return null;
        }

        if (m_currentTunnelIndex >= m_tunnels.Count - 1)
        {
            CentipedeMovement.m_useTrainSpeed = false;
            CentipedeBoss.m_dropLava = false;
            StartCoroutine(ReenterArena());
        }
        else
        {
            m_currentTunnelIndex++;
            StartCoroutine(TunnelAttack());
        }
    }

    private IEnumerator ReenterArena()
    {
        m_centipedeHealth.ActivateSection(false, CentipedeHealth.ESegmentType.head);
        CentipedeMovement.SetTargets(new List<Transform> { m_tunnels[0].m_tunnelStart});

        while (!CentipedeMovement.m_atTarget)
        {
            yield return null;
        }

        CompleteBehaviour();
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
        CentipedeMovement.m_seekingTarget = false;
    }

    public override void Reset()
    {
        base.Reset();
        m_currentTunnelIndex = 0;
    }
}
