using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLaserAttack : CentipedeBehaviour
{
    public List<CentipedeBodySegment> m_bodySegments;

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        Debug.Log("Laser attack started");

        CentipedeMovement.m_seekingTarget = true;
        StartCoroutine(FireLaserGroups());
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
        Debug.Log("Laser attack finished");

        CentipedeMovement.m_seekingTarget = false;
    }

    private IEnumerator FireLaserGroups()
    {
        for (int i = 0; i < CentipedeBoss.m_settings.m_timesLasersFired; i++)
        {
            FireLasers();

            yield return new WaitForSeconds(CentipedeBoss.m_settings.m_laserDuration + CentipedeBoss.m_settings.m_timeBetweenLasers);
        }

        CompleteBehaviour();
    }

    private void FireLasers()
    {
        // Keep list of segments to avoid firing one twice
        List<int> segments = new List<int> { 0, 1, 2, 3, 4 };

        // Repeat firing lasers until number is reached
        for (int i = 0; i < CentipedeBoss.m_settings.m_lasersAtOnce; i++)
        {
            // Get random index of indexes
            int randomSegment = Random.Range(0, segments.Count);
            
            // Find the body segment and fire laser
            CentipedeBodySegment segmentToFire = m_bodySegments[segments[randomSegment]];
            segmentToFire.FireLaserFor(CentipedeBoss.m_settings.m_laserDuration);

            // Remove this one from the list, so we don't fire it twice
            segments.RemoveAt(randomSegment);
        }
    }

    public override void Reset()
    {
        base.Reset();
    }
}
