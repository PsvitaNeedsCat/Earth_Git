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
    public Transform m_mesh;

    private CentipedeHealth m_centipedeHealth;
    private float m_timeFiredFor = 0.0f;
    private float m_timeSinceLastFire = 0.0f;

    private void Awake()
    {
        m_centipedeHealth = GetComponent<CentipedeHealth>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        StartCoroutine(BurrowDown());
    }

    private IEnumerator BurrowDown()
    {
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

        // m_mesh.transform.Rotate(m_mesh.transform.right, 90.0f);
        // m_animations.TailAttackStart();

        m_firer.transform.DOBlendableLocalRotateBy(Vector3.up * CentipedeBoss.m_settings.m_rotationSpeed * 100.0f, CentipedeBoss.m_settings.m_firingDuration);
        StartCoroutine(FireProjectiles());
    }

    private IEnumerator BurrowUp()
    {
        Debug.Log("Starting burrow up");

        // m_mesh.transform.Rotate(m_mesh.transform.right, -90.0f);
        // m_animations.TailAttackEnd();

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
        m_centipedeHealth.ActivateSection(true, 6);

        while (m_timeFiredFor < CentipedeBoss.m_settings.m_firingDuration)
        {
            m_timeFiredFor += Time.deltaTime;
            m_timeSinceLastFire += Time.deltaTime;

            bool tailDamaged = m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.tail);
            float fireDelay = (tailDamaged) ? CentipedeBoss.m_settings.m_fireDelayDamaged : CentipedeBoss.m_settings.m_fireDelay;

            if (m_timeSinceLastFire >= fireDelay)
            {
                m_firer.FireAll(tailDamaged);
                m_timeSinceLastFire -= fireDelay;
            }

            yield return null;
        }

        m_centipedeHealth.ActivateSection(false, 6);

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
