using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLaserAttack : CentipedeBehaviour
{
    public List<CentipedeBodySegment> m_bodySegments;
    public List<Transform> m_movePoints;
    private CentipedeHealth m_centipedeHealth;

    private void Awake()
    {
        m_centipedeHealth = GetComponent<CentipedeHealth>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        CentipedeMovement.m_seekingTarget = true;
        CentipedeMovement.m_loopTargets = true;
        CentipedeMovement.SetTargets(m_movePoints);
        m_centipedeHealth.ActivateSection(true, CentipedeHealth.ESegmentType.body);
        StartCoroutine(FireLaserGroups());
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();

        CentipedeMovement.m_seekingTarget = false;
        CentipedeMovement.m_loopTargets = false;
    }

    private IEnumerator FireLaserGroups()
    {
        for (int i = 0; i < CentipedeBoss.m_settings.m_timesLasersFired; i++)
        {
            bool bodyDamaged = m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body);
            float laserDuration = (bodyDamaged) ? CentipedeBoss.m_settings.m_laserDurationDamaged : CentipedeBoss.m_settings.m_laserDuration;
            float timeBetween = (bodyDamaged) ? CentipedeBoss.m_settings.m_timeBetweenLasersDamaged : CentipedeBoss.m_settings.m_timeBetweenLasers;

            Debug.Log("Firing laser group");
            StartCoroutine(FireLasers(laserDuration));

            yield return new WaitForSeconds(laserDuration + timeBetween + CentipedeBoss.m_settings.m_laserAnticipation);
        }

        m_centipedeHealth.ActivateSection(false, CentipedeHealth.ESegmentType.body);

        CompleteBehaviour();
    }

    private IEnumerator FireLasers(float _laserDuration)
    {
        // Keep list of segments to avoid firing one twice
        List<int> segments = new List<int> { 0, 1, 2, 3, 4 };

        int lasersAtOnce = (m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body)) ? CentipedeBoss.m_settings.m_lasersAtOnceDamaged : CentipedeBoss.m_settings.m_lasersAtOnce;

        List<CentipedeBodySegment> segmentsToFire = new List<CentipedeBodySegment>();

        // Find body segments to fire
        for (int i = 0; i < lasersAtOnce; i++)
        {
            // Get random index of indexes
            int randomSegment = Random.Range(0, segments.Count);
            
            // Find the body segment
            CentipedeBodySegment segmentToFire = m_bodySegments[segments[randomSegment]];
            segmentsToFire.Add(segmentToFire);

            // Remove this one from the list, so we don't fire it twice
            segments.RemoveAt(randomSegment);
        }

        Debug.Log("Chose " + segmentsToFire.Count + " lasers");

        for (int i = 0; i < segmentsToFire.Count; i++)
        {
            Debug.Log("Executing fire warning");
            segmentsToFire[i].FireWarning(CentipedeBoss.m_settings.m_laserAnticipation);
        }

        yield return new WaitForSeconds(CentipedeBoss.m_settings.m_laserAnticipation);

        for (int i = 0; i < segmentsToFire.Count; i++)
        {
            Debug.Log("Firing laser");
            segmentsToFire[i].FireLaserFor(_laserDuration);
        }
    }

    public override void Reset()
    {
        base.Reset();
    }
}
