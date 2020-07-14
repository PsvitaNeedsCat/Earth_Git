using System.Collections;
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
    public GameObject m_trainAudio;
    public static bool m_charging = false;
    public static bool m_stunned = false;
    public CentipedeHead m_head;

    private int m_currentTunnelIndex = 0;
    private int m_chunksHit = 0;
    private CentipedeHealth m_centipedeHealth;


    private void Awake()
    {
        m_centipedeHealth = GetComponent<CentipedeHealth>();
        m_charging = false;
        m_stunned = false;
        Debug.Log("Stunned is false");
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();
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
        m_trainAudio.SetActive(true);
        m_animations.ChargeStart();
        StartCoroutine(TunnelAttack());
    }

    private IEnumerator TunnelAttack()
    {
        m_charging = true;
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
            m_charging = false;
            m_animations.ChargeEnd();
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
        m_trainAudio.SetActive(false);
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
        m_charging = false;
        CentipedeMovement.m_useTrainSpeed = false;
        CentipedeBoss.m_dropLava = false;
        m_trainAudio.SetActive(false);
    }

    public override void Reset()
    {
        base.Reset();
        m_currentTunnelIndex = 0;
        m_chunksHit = 0;
        m_stunned = false;
        Debug.Log("Stunned is false");
    }

    public void HitByChunk()
    {
        m_chunksHit++;

        if (m_chunksHit >= CentipedeBoss.m_settings.m_chunksToStun)
        {
            StartCoroutine(StunFor(CentipedeBoss.m_settings.m_stunnedFor));
        }
    }

    private IEnumerator StunFor(float _forSeconds)
    {
        m_stunned = true;
        Debug.Log("Stunned is true");
        m_head.DisableCollider();
        CentipedeMovement.m_seekingTarget = false;
        m_charging = false;
        m_centipedeHealth.ActivateSection(true, CentipedeHealth.ESegmentType.head);
        m_animations.Stunned();
        yield return new WaitForSeconds(_forSeconds);
        StartCoroutine(Recover());
    }

    private IEnumerator Recover()
    {
        Debug.Log("Recovering");
        Debug.Log("Stunned is false");
        m_animations.Recovered();
        yield return new WaitForSeconds(0.1f);
        m_stunned = false;
        m_centipedeHealth.ActivateSection(false, CentipedeHealth.ESegmentType.head);
        m_head.EnableCollider();
        CompleteBehaviour();
    }

    public void OnDamaged()
    {
        Debug.Log("Damaged");
        StopAllCoroutines();
        StartCoroutine(Recover());
    }

}
