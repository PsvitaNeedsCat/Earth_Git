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
        // Move to the pre-burrow point
        CentipedeMovement.SetTargets(new List<Transform> { m_preBurrowPoint });
        CentipedeMovement.m_seekingTarget = true;
        while (!CentipedeMovement.m_atTarget) yield return null;

        // Burrow down
        CentipedeMovement.m_seekingTarget = false;
        CentipedeMovement.BurrowDown(m_burrowDownPoints);
        while (CentipedeMovement.m_burrowing) yield return null;

        // Start tail attack
        CentipedeMovement.m_burrowing = false;
        m_animations.TailAttackStart();

        // Move mesh to undo animation position and rotation changes
        m_mesh.transform.localPosition = m_mesh.transform.localPosition + Vector3.up;
        m_mesh.transform.Rotate(m_mesh.transform.right, -90.0f);

        // Rotate the firing object
        m_firer.transform.DOBlendableLocalRotateBy(Vector3.up * CentipedeBoss.m_settings.m_rotationSpeed * 100.0f, CentipedeBoss.m_settings.m_firingDuration);
        StartCoroutine(FireProjectiles());
    }

    private IEnumerator BurrowUp()
    {
        m_animations.TailAttackEnd();
        
        // Burrow up
        CentipedeMovement.BurrowUp(m_burrowUpPoints);
        while (CentipedeMovement.m_burrowing) yield return null;

        // Undo the position and rotation changes from the tail
        m_mesh.transform.Rotate(m_mesh.transform.right, 90.0f);
        m_mesh.transform.localPosition = m_mesh.transform.localPosition - Vector3.up;
        CentipedeMovement.m_burrowing = false;

        CompleteBehaviour();
    }

    private IEnumerator FireProjectiles()
    {
        // Activate the tail section
        m_centipedeHealth.ActivateSection(true, 6);

        // Fire for a duration
        while (m_timeFiredFor < CentipedeBoss.m_settings.m_firingDuration)
        {
            m_timeFiredFor += Time.deltaTime;
            m_timeSinceLastFire += Time.deltaTime;

            bool tailDamaged = m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.tail);
            float fireDelay = (tailDamaged) ? CentipedeBoss.m_settings.m_fireDelayDamaged : CentipedeBoss.m_settings.m_fireDelay;

            // If enough time has passed, fire
            if (m_timeSinceLastFire >= fireDelay)
            {
                m_firer.FireAll(tailDamaged);
                m_timeSinceLastFire -= fireDelay;
            }

            yield return null;
        }

        // Deactivate the tail section
        m_centipedeHealth.ActivateSection(false, 6);

        StartCoroutine(BurrowUp());
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
    }

    public override void Reset()
    {
        base.Reset();
        m_timeFiredFor = 0.0f;
        m_timeSinceLastFire = 0.0f;
    }
}
