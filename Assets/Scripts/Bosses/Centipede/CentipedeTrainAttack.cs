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
    public static bool s_charging = false;
    public static bool s_stunned = false;
    public CentipedeHead m_head;
    public GameObject m_activeEffects;

    private int m_currentTunnelIndex = 0;
    private int m_chunksHit = 0;
    private CentipedeHealth m_centipedeHealth;
    [SerializeField] private StunnedStars m_stunnedStars = null;

    private void Awake()
    {
        m_centipedeHealth = GetComponent<CentipedeHealth>();
        s_charging = false;
        s_stunned = false;
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
        CentipedeMovement.s_seekingTarget = true;

        while (!CentipedeMovement.s_atTarget)
        {
            yield return null;
        }

        // Start charging
        CentipedeMovement.s_useTrainSpeed = true;
        CentipedeBoss.s_dropLava = true;
        m_trainAudio.SetActive(true);
        m_animations.ChargeStart();
        StartCoroutine(TunnelAttack());
    }

    private IEnumerator TunnelAttack()
    {
        s_charging = true;
        TunnelDef currentTunnel = m_tunnels[m_currentTunnelIndex];
        CentipedeMovement.SetTargets(new List<Transform> { currentTunnel.m_tunnelEnd, currentTunnel.m_tunnelStart, currentTunnel.m_tunnelTarget, currentTunnel.m_nextCorner });
        m_activeEffects.SetActive(true);

        // Wait for centipede to reach a target
        while (!CentipedeMovement.s_atTarget)
        {
            yield return null;
        }

        // If done with all tunnels, return to arena
        if (m_currentTunnelIndex >= m_tunnels.Count - 1)
        {
            CentipedeMovement.s_useTrainSpeed = false;
            CentipedeBoss.s_dropLava = false;
            s_charging = false;
            m_animations.ChargeEnd();
            StartCoroutine(ReenterArena());
        }
        // Otherwise, go to the next tunnel
        else
        {
            m_currentTunnelIndex++;
            StartCoroutine(TunnelAttack());
        }
    }

    // Move back into the arena
    private IEnumerator ReenterArena()
    {
        m_activeEffects.SetActive(false);
        m_trainAudio.SetActive(false);
        CentipedeMovement.SetTargets(new List<Transform> { m_tunnels[0].m_tunnelStart});

        while (!CentipedeMovement.s_atTarget)
        {
            yield return null;
        }

        CompleteBehaviour();
    }

    // Reset variables upon completing behaviour
    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
        CentipedeMovement.s_seekingTarget = false;
        s_charging = false;
        CentipedeMovement.s_useTrainSpeed = false;
        CentipedeBoss.s_dropLava = false;
        m_trainAudio.SetActive(false);
    }

    // Reset variables for the next train attack
    public override void Reset()
    {
        base.Reset();
        m_currentTunnelIndex = 0;
        m_chunksHit = 0;
        s_stunned = false;
        // Debug.Log("Stunned is false");
    }

    // When hit by a chunk during the train attack
    public void HitByChunk()
    {
        m_chunksHit++;

        // If hit by enough chunks, get stunned
        if (m_chunksHit >= CentipedeBoss.s_settings.m_chunksToStun)
        {
            StartCoroutine(StunFor(CentipedeBoss.s_settings.m_stunnedFor));
        }
    }

    // 
    private IEnumerator StunFor(float _forSeconds)
    {
        // Get stunned for a duration
        s_stunned = true;
        m_head.DisableCollider();
        m_activeEffects.SetActive(false);
        CentipedeMovement.s_seekingTarget = false;
        s_charging = false;
        m_centipedeHealth.ActivateSection(true, 0);
        MessageBus.TriggerEvent(EMessageType.vulnerableStart);

        m_animations.Stunned();
        if (m_stunnedStars)
        {
            m_stunnedStars.Init(_forSeconds);
        }
        yield return new WaitForSeconds(_forSeconds);

        // After duration, recover
        StartCoroutine(Recover());
    }

    private IEnumerator Recover()
    {
        // Wait a short amount for the stunned animation to end
        m_animations.Recovered();
        yield return new WaitForSeconds(0.1f);

        // Complete behaviour
        s_stunned = false;
        m_centipedeHealth.ActivateSection(false, 0);
        MessageBus.TriggerEvent(EMessageType.vulnerableEnd);

        m_head.EnableCollider();
        CompleteBehaviour();
    }

    // If damaged, exit out of stunned animation
    public void OnDamaged()
    {
        m_stunnedStars.ForceStop();
        StopAllCoroutines();
        StartCoroutine(Recover());
    }
}
