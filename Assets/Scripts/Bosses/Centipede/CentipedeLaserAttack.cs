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

            FireLasers(laserDuration);

            yield return new WaitForSeconds(laserDuration + timeBetween);
        }

        CompleteBehaviour();
    }

    private void FireLasers(float _laserDuration)
    {
        // Keep list of segments to avoid firing one twice
        List<int> segments = new List<int> { 0, 1, 2, 3, 4 };

        int lasersAtOnce = (m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body)) ? CentipedeBoss.m_settings.m_lasersAtOnceDamaged : CentipedeBoss.m_settings.m_lasersAtOnce;

        // Repeat firing lasers until number is reached
        for (int i = 0; i < lasersAtOnce; i++)
        {
            // Get random index of indexes
            int randomSegment = Random.Range(0, segments.Count);
            
            // Find the body segment and fire laser
            CentipedeBodySegment segmentToFire = m_bodySegments[segments[randomSegment]];
            segmentToFire.FireLaserFor(_laserDuration);

            // Remove this one from the list, so we don't fire it twice
            segments.RemoveAt(randomSegment);
        }
    }

    public override void Reset()
    {
        base.Reset();
    }
}
