using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Cobra chase attack behaviour
public class CobraChase : CobraBehaviour
{
    public Transform m_mesh;
    public Transform m_arenaCenter;
    public GameObject m_stompHurtbox;
    public GameObject m_crystal;
    public ProjectileDropShadow m_dropShadow;

    private int m_timesJumped = 0;
    private Player m_playerRef;
    private Transform m_moveTransform; // Use this transform to move the cobra
    private bool m_dead = false;

    protected override void Awake()
    {
        base.Awake();

        // Initialise variables
        m_playerRef = FindObjectOfType<Player>();
        m_moveTransform = transform.parent;

        m_dropShadow.SetDropShadow(false);
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        StartCoroutine(JumpUp());
    }

    // Performs the jump up behaviour in order to start the chase attack
    private IEnumerator JumpUp()
    {
        m_animations.EnterPot();

        yield return new WaitForSeconds(0.5f);

        // Turn on the drop shadow
        m_dropShadow.gameObject.SetActive(true);
        m_dropShadow.SetDropShadow(true);

        m_stompHurtbox.SetActive(true);

        StartCoroutine(Chase());
    }

    // Returns the direction from the cobra to the player (ignoring changes on the y axis)
    private Vector3 DirToPlayer()
    {
        Vector3 toPlayer = m_playerRef.transform.position - m_moveTransform.position;
        toPlayer.y = 0.0f;
        return Vector3.ClampMagnitude(toPlayer, 1.0f);
    }

    // Chase after the player for a number of jumps, then die
    private IEnumerator Chase()
    {
        float waitTime = CobraBoss.s_settings.m_jumpDelay + CobraBoss.s_settings.m_chaseJumpRaiseTime + CobraBoss.s_settings.m_chaseJumpAimTime + CobraBoss.s_settings.m_chaseJumpFallTime;

        // Jump after the player for a number of jumps
        for (int i = 0; i < CobraBoss.s_settings.m_jumpsBeforeDeath; i++)
        {
            yield return new WaitForSeconds(waitTime);
            ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.small);
            MessageBus.TriggerEvent(EMessageType.cobraPotThud);

            StartCoroutine(Jump());
        }

        // Wait for last jump to complete before dying
        yield return new WaitForSeconds(waitTime);

        Death();

        StartCoroutine(BossHelper.SlowTimeFor(0.1f, 0.25f, 0.5f, 0.25f));

        yield return new WaitForSeconds(2.0f);

        transform.parent.gameObject.SetActive(false);

        // Die and activate the crystal
        m_crystal.SetActive(true);
        m_crystal.GetComponentInChildren<Crystal>().Collected(m_playerRef);
    }

    private void Update()
    {
        if (!(m_currentState == EBehaviourState.running) || m_dead)
        {
            return;
        }

        // Look at the player
        Vector3 playerPos = m_playerRef.transform.position;
        playerPos.y = m_moveTransform.position.y;
        m_moveTransform.LookAt(playerPos);
    }

    // Jump towards the player
    private IEnumerator Jump()
    {
        // Jump up into the air
        m_moveTransform.DOMove(m_moveTransform.position + Vector3.up * CobraBoss.s_settings.m_chaseJumpHeight, CobraBoss.s_settings.m_chaseJumpRaiseTime).SetEase(CobraBoss.s_settings.m_chaseRaiseCurve);

        yield return new WaitForSeconds(CobraBoss.s_settings.m_chaseJumpRaiseTime);

        // Move to a tile (tile in front of player?)
        float aimTimer = 0.0f;

        while (aimTimer < CobraBoss.s_settings.m_chaseJumpAimTime)
        {
            aimTimer += Time.deltaTime;
            
            Vector3 dir = DirToPlayer();
            m_moveTransform.position = Vector3.Lerp(m_moveTransform.position, m_moveTransform.position + dir, Time.deltaTime * CobraBoss.s_settings.m_chaseSpeed);

            yield return null;
        }

        // Slam down on that tile
        Vector3 landingPos = m_moveTransform.position;
        landingPos.y = m_arenaCenter.position.y - 0.49f;
        m_moveTransform.DOMove(landingPos, CobraBoss.s_settings.m_chaseJumpFallTime).SetEase(CobraBoss.s_settings.m_chaseFallCurve).OnComplete(() => 
        {
            ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);
            MessageBus.TriggerEvent(EMessageType.cobraPotBigThud);
            EffectsManager.SpawnEffect(EffectsManager.EEffectType.cobraPotLand, landingPos, transform.rotation, Vector3.one, 1.0f);
        });
    }

    private void Death()
    {
        m_dead = true;

        MessageBus.TriggerEvent(EMessageType.cobraDeath);
        MessageBus.TriggerEvent(EMessageType.potDestroyed);
        EffectsManager.SpawnEffect(EffectsManager.EEffectType.cobraPotBreak, m_moveTransform.position, m_moveTransform.rotation, Vector3.one * 2.5f, 4.0f);
        // Destroy(transform.parent.parent.gameObject);
        m_dropShadow.SetDropShadow(false);
        m_dropShadow.m_dropShadow.SetActive(false);
        m_mesh.localScale = Vector3.zero;
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
    }

    public override void Reset()
    {
        base.Reset();
    }
}
