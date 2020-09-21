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
    public GameObject m_shields;

    private CentipedeHealth m_centipedeHealth;
    private float m_timeFiredFor = 0.0f;
    private float m_timeSinceLastFire = 0.0f;
    private Quaternion m_oldRotation;

    private void Awake()
    {
        m_centipedeHealth = GetComponent<CentipedeHealth>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        StartCoroutine(BurrowDown());

        // Reactivate disabled shields
        foreach (Transform child in m_shields.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private IEnumerator BurrowDown()
    {
        // Move to the pre-burrow point
        CentipedeMovement.SetTargets(new List<Transform> { m_preBurrowPoint });
        CentipedeMovement.s_seekingTarget = true;
        while (!CentipedeMovement.s_atTarget) yield return null;

        // Burrow down
        CentipedeMovement.s_seekingTarget = false;
        CentipedeMovement.BurrowDown(m_burrowDownPoints);
        while (CentipedeMovement.s_burrowing) yield return null;

        // Start tail attack
        CentipedeMovement.s_burrowing = false;
        m_animations.TailAttackStart();

        // Move mesh to undo animation position and rotation changes
        m_mesh.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        m_mesh.transform.localPosition -= Vector3.forward * 1.5f;

        // Rotate the firing object
        m_firer.transform.localPosition = new Vector3(0.0f, 1.0f, -1.0f);
        m_firer.transform.localRotation = Quaternion.identity;
        m_firer.transform.DOBlendableLocalRotateBy(Vector3.forward * CentipedeBoss.s_settings.m_rotationSpeed * 10.0f, CentipedeBoss.s_settings.m_firingDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);

        m_shields.SetActive(true);
        StartCoroutine(FireProjectiles());
    }

    private IEnumerator BurrowUp()
    {
        m_animations.TailAttackEnd();

        // Burrow up
        m_shields.SetActive(false);

        yield return new WaitForSeconds(0.8f);

        m_mesh.transform.localPosition += Vector3.forward * 3.0f;
        m_firer.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        m_mesh.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        
        CentipedeMovement.BurrowUp(m_burrowUpPoints);

        yield return new WaitForSeconds(3.0f);

        m_mesh.transform.localPosition -= Vector3.forward * 1.5f;

        while (CentipedeMovement.s_burrowing) yield return null;

        CentipedeMovement.s_burrowing = false;

        CompleteBehaviour();
    }

    private IEnumerator FireProjectiles()
    {
        // Activate the tail section
        m_centipedeHealth.ActivateSection(true, 6);
        MessageBus.TriggerEvent(EMessageType.vulnerableStart);

        // Fire for a duration
        while (m_timeFiredFor < CentipedeBoss.s_settings.m_firingDuration)
        {
            m_timeFiredFor += Time.deltaTime;
            m_timeSinceLastFire += Time.deltaTime;

            bool tailDamaged = m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.tail);
            float fireDelay = (tailDamaged) ? CentipedeBoss.s_settings.m_fireDelayDamaged : CentipedeBoss.s_settings.m_fireDelay;

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
        MessageBus.TriggerEvent(EMessageType.vulnerableEnd);

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
