using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraChase : CobraBehaviour
{
    public Transform m_mesh;
    public GameObject m_dropShadow;
    public Transform m_arenaCenter;
    public GameObject m_stompHurtbox;
    public GameObject m_crystal;

    private int m_timesJumped = 0;
    private Player m_playerRef;
    private Transform m_moveTransform; // Use this transform to move the cobra

    private void Awake()
    {
        m_playerRef = FindObjectOfType<Player>();
        m_moveTransform = transform.parent;
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        StartCoroutine(JumpUp());
    }

    private IEnumerator JumpUp()
    {
        m_dropShadow.SetActive(true);
        
        bool landed = false;

        m_mesh.transform.DOPunchPosition(Vector3.up * CobraBoss.m_settings.m_bigJumpHeight, CobraBoss.m_settings.m_bigJumpDuration, vibrato: 0, elasticity: 0).SetEase(Ease.OutCubic);
        m_moveTransform.DOMove(m_arenaCenter.position, CobraBoss.m_settings.m_bigJumpDuration).OnComplete(() => landed = true);
        
        while (!landed)
        {
            UpdateDropShadow();
            yield return null;
        }

        m_stompHurtbox.SetActive(true);
        m_dropShadow.SetActive(false);

        StartCoroutine(Chase());
    }

    private void UpdateDropShadow()
    {
        float dist = Mathf.Abs(m_mesh.transform.position.y - m_arenaCenter.position.y);

        float quot = Mathf.Clamp01(dist / CobraBoss.m_settings.m_bigJumpHeight);
        m_dropShadow.transform.localScale = Vector3.one * Mathf.Lerp(CobraBoss.m_settings.m_dropShadowMaxScale, CobraBoss.m_settings.m_dropShadowMinScale, quot);
    }

    private Vector3 DirToPlayer()
    {
        Vector3 toPlayer = m_playerRef.transform.position - transform.position;
        toPlayer.y = 0.0f;
        return Vector3.ClampMagnitude(toPlayer, 1.0f);
    }

    private IEnumerator Chase()
    {
        for (int i = 0; i < CobraBoss.m_settings.m_jumpsBeforeDeath; i++)
        {
            yield return new WaitForSeconds(CobraBoss.m_settings.m_jumpDuration + CobraBoss.m_settings.m_jumpDelay);

            Jump();
        }

        // Wait for last jump to complete before dying
        yield return new WaitForSeconds(CobraBoss.m_settings.m_jumpDuration);

        m_crystal.SetActive(true);
        Death();
    }

    private void Update()
    {
        if (!(m_currentState == EBehaviourState.running))
        {
            return;
        }

        Vector3 playerPos = m_playerRef.transform.position;
        playerPos.y = transform.position.y;
        m_moveTransform.LookAt(playerPos);
    }

    private void Jump()
    {
        Vector3 dir = DirToPlayer();

        m_moveTransform.DOMove(transform.position + dir, CobraBoss.m_settings.m_jumpDuration);
        m_mesh.DOPunchPosition(Vector3.up * 1.0f, CobraBoss.m_settings.m_jumpDuration).SetEase(Ease.InOutElastic);
    }

    private void Death()
    {
        Destroy(transform.parent.parent.gameObject);
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
