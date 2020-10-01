using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Cobra chase attack behaviour
public class CobraChase : CobraBehaviour
{
    public Transform m_mesh;
    public GameObject m_fallingDropShadow;
    public Transform m_arenaCenter;
    public GameObject m_stompHurtbox;
    public GameObject m_crystal;
    public GameObject m_chasingDropShadow;

    private int m_timesJumped = 0;
    private Player m_playerRef;
    private Transform m_moveTransform; // Use this transform to move the cobra

    protected override void Awake()
    {
        base.Awake();

        // Initialise variables
        m_playerRef = FindObjectOfType<Player>();
        m_moveTransform = transform.parent;
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        StartCoroutine(JumpUp());
    }

    // Performs the jump up behaviour in order to start the chase attack
    private IEnumerator JumpUp()
    {
        m_animations.ExitPot();

        yield return new WaitForSeconds(2.0f);

        // Turn on the drop shadow
        m_fallingDropShadow.SetActive(true);
        
        bool landed = false;

        // Jump up in the air
        m_mesh.transform.DOPunchPosition(Vector3.up * CobraBoss.s_settings.m_bigJumpHeight, CobraBoss.s_settings.m_bigJumpDuration, vibrato: 0, elasticity: 0).SetEase(Ease.OutCubic);
        m_moveTransform.DOMove(m_arenaCenter.position - Vector3.up * 0.49f, CobraBoss.s_settings.m_bigJumpDuration).OnComplete(() => landed = true);

        // Update the drop shadow until we've landed
        while (!landed)
        {
            UpdateDropShadow();
            yield return null;
        }

        MessageBus.TriggerEvent(EMessageType.cobraPotBigThud);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);

        // Disable drop shadow
        m_fallingDropShadow.SetActive(false);
        m_stompHurtbox.SetActive(true);
        m_chasingDropShadow.SetActive(true);

        StartCoroutine(Chase());
    }

    private void UpdateDropShadow()
    {
        // Update scale of drop shadow based on distance to the cobra
        float dist = Mathf.Abs(m_mesh.transform.position.y - m_arenaCenter.position.y);
        float quot = Mathf.Clamp01(dist / CobraBoss.s_settings.m_bigJumpHeight);
        m_fallingDropShadow.transform.localScale = Vector3.one * Mathf.Lerp(CobraBoss.s_settings.m_dropShadowMaxScale, CobraBoss.s_settings.m_dropShadowMinScale, quot);
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
        // Jump after the player for a number of jumps
        for (int i = 0; i < CobraBoss.s_settings.m_jumpsBeforeDeath; i++)
        {
            ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.small);
            MessageBus.TriggerEvent(EMessageType.cobraPotThud);
            yield return new WaitForSeconds(CobraBoss.s_settings.m_jumpDuration + CobraBoss.s_settings.m_jumpDelay);

            Jump();
        }

        // Wait for last jump to complete before dying
        yield return new WaitForSeconds(CobraBoss.s_settings.m_jumpDuration);

        // Die and activate the crystal
        m_crystal.SetActive(true);
        Death();
    }

    private void Update()
    {
        if (!(m_currentState == EBehaviourState.running))
        {
            return;
        }

        // Look at the player
        Vector3 playerPos = m_playerRef.transform.position;
        playerPos.y = m_moveTransform.position.y;
        m_moveTransform.LookAt(playerPos);
    }

    // Jump towards the player
    private void Jump()
    {
        Vector3 dir = DirToPlayer();

        // Debug.Log("Cobra chase moving to player: " + dir);

        m_moveTransform.DOMove(m_moveTransform.position + dir, CobraBoss.s_settings.m_jumpDuration);
        m_mesh.DOPunchPosition(Vector3.up * 1.0f, CobraBoss.s_settings.m_jumpDuration).SetEase(Ease.InOutElastic);
    }

    private void Death()
    {
        MessageBus.TriggerEvent(EMessageType.cobraDeath);
        MessageBus.TriggerEvent(EMessageType.potDestroyed);
        // Destroy(transform.parent.parent.gameObject);
        transform.parent.gameObject.SetActive(false);
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
