using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

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
    [SerializeField] private GameObject m_movingEffects = null;
    [SerializeField] private GameObject m_fireEffects = null;
    [SerializeField] private List<GameObject> m_tunnelLights;

    private int m_currentTunnelIndex = 0;
    private int m_chunksHit = 0;
    private CentipedeHealth m_centipedeHealth;
    [SerializeField] private StunnedStars m_stunnedStars = null;
    [SerializeField] private DisableScreenShakeListener m_chargingScreenShake = null;

    private void Awake()
    {
        m_centipedeHealth = GetComponent<CentipedeHealth>();
        s_charging = false;
        s_stunned = false;

        m_chargingScreenShake.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        m_chargingScreenShake.StopScreenShake();
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
        m_chargingScreenShake.ResumeScreenShake();
        m_animations.ChargeStart();
        StartCoroutine(TunnelAttack());
    }

    private IEnumerator TunnelAttack()
    {
        s_charging = true;
        TunnelDef currentTunnel = m_tunnels[m_currentTunnelIndex];
        CentipedeMovement.SetTargets(new List<Transform> { currentTunnel.m_tunnelEnd, currentTunnel.m_tunnelStart, currentTunnel.m_tunnelTarget, currentTunnel.m_nextCorner });
        m_movingEffects.SetActive(true);
        if (!m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.head))
        {
            m_fireEffects.SetActive(true);
        }

        // Wait for centipede to reach a target
        while (!CentipedeMovement.s_atTarget)
        {
            int currentTargetIndex = CentipedeMovement.GetCurrentTargetIndex();

            // If in tunnel
            if (currentTargetIndex == 1)
            {
                m_tunnelLights[m_currentTunnelIndex].SetActive(true);
                float distToEntrance = (m_head.transform.position - m_tunnelLights[m_currentTunnelIndex].transform.position).magnitude;
                float lightQuotient = Mathf.Clamp01(1.0f - (distToEntrance / 7.0f));
                Material lightMat = m_tunnelLights[m_currentTunnelIndex].GetComponent<MeshRenderer>().material;
                Color lightColor = lightMat.color;
                lightColor.a = lightQuotient;
                lightMat.color = lightColor;
            }
            else
            {
                m_tunnelLights[m_currentTunnelIndex].SetActive(false);
            }

            yield return null;
        }

        // If done with all tunnels, return to arena
        if (m_currentTunnelIndex >= m_tunnels.Count - 1)
        {
            CentipedeMovement.s_useTrainSpeed = false;
            CentipedeBoss.s_dropLava = false;
            s_charging = false;
            m_animations.ChargeEnd();
            m_chargingScreenShake.StopScreenShake();
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
        m_movingEffects.SetActive(false);
        m_fireEffects.SetActive(false);
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
        m_fireEffects.SetActive(false);
        m_movingEffects.SetActive(false);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.centipedeHitChunk);
        StartCoroutine(StopScreenShakeDelay());
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

    // Stops the screen shake after a delay
    private IEnumerator StopScreenShakeDelay()
    {
        yield return new WaitForSeconds(0.1f);

        m_chargingScreenShake.StopScreenShake();
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
