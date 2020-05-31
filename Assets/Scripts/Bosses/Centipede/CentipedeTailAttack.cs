using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CentipedeTailAttack : CentipedeBehaviour
{
    public CentipedeTailFirer m_firer;
    public Transform m_preBurrowPoint;
    public List<Transform> m_burrowDownPoints;
    public List<Transform> m_burrowUpPoints;

    private float m_timeFiredFor = 0.0f;
    private float m_timeSinceLastFire = 0.0f;

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        Debug.Log("Tail started");
        StartCoroutine(BurrowDown());
    }

    private IEnumerator BurrowDown()
    {
        Debug.Log("Starting burrow down");

        CentipedeMovement.SetTargets(new List<Transform> { m_preBurrowPoint });
        CentipedeMovement.m_seekingTarget = true;
        while (!CentipedeMovement.m_atTarget) yield return null;
        CentipedeMovement.m_seekingTarget = false;

        // CentipedeMovement.m_burrowing = true;
        CentipedeMovement.BurrowDown(m_burrowDownPoints);
        // yield return new WaitForSeconds(CentipedeBoss.m_settings.m_burrowDuration * 8.0f);
        while (CentipedeMovement.m_burrowing) yield return null;

        //while (!CentipedeMovement.m_atTarget) yield return null;
        CentipedeMovement.m_burrowing = false;

        m_firer.transform.DOBlendableLocalRotateBy(Vector3.up * CentipedeBoss.m_settings.m_rotationSpeed * 100.0f, CentipedeBoss.m_settings.m_firingDuration);
        StartCoroutine(FireProjectiles());
    }

    private IEnumerator BurrowUp()
    {
        Debug.Log("Starting burrow up");

        // CentipedeMovement.m_burrowing = true;
        CentipedeMovement.BurrowUp(m_burrowUpPoints);
        // yield return new WaitForSeconds(CentipedeBoss.m_settings.m_burrowDuration * 8.0f);
        while (CentipedeMovement.m_burrowing) yield return null;
        // while (CentipedeMovement.m_burrowed) yield return null;
        CentipedeMovement.m_burrowing = false;

        CompleteBehaviour();
    }

    private IEnumerator FireProjectiles()
    {
        Debug.Log("Starting firing projectiles");

        while (m_timeFiredFor < CentipedeBoss.m_settings.m_firingDuration)
        {
            m_timeFiredFor += Time.deltaTime;
            m_timeSinceLastFire += Time.deltaTime;

            if (m_timeSinceLastFire >= CentipedeBoss.m_settings.m_fireDelay)
            {
                m_firer.FireAll();
                m_timeSinceLastFire -= CentipedeBoss.m_settings.m_fireDelay;
            }

            yield return null;
        }

        StartCoroutine(BurrowUp());
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
        Debug.Log("Tail finished");
    }

    public override void Reset()
    {
        base.Reset();
        m_timeFiredFor = 0.0f;
        m_timeSinceLastFire = 0.0f;
    }
}
