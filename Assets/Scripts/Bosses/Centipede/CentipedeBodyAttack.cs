using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBodyAttack : CentipedeBehaviour
{
    public List<CentipedeSegmentFirer> m_segmentFirers;
    public List<Transform> m_movePoints;

    private CentipedeHealth m_centipedeHealth;

    private void Awake()
    {
        m_centipedeHealth = GetComponent<CentipedeHealth>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        // Move around the edge of the arena
        CentipedeMovement.m_seekingTarget = true;
        CentipedeMovement.m_loopTargets = true;
        CentipedeMovement.SetTargets(m_movePoints);

        // Start firing projectiles
        StartCoroutine(FiringSequence());
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();

        // Stop pathfinding
        CentipedeMovement.m_seekingTarget = false;
        CentipedeMovement.m_loopTargets = false;
    }

    private IEnumerator FiringSequence()
    {
        // Delay before firing first attack
        yield return new WaitForSeconds(CentipedeBoss.m_settings.m_bodyAttackStartDelay);

        // Determine how many times to fire a line of projectiles
        int numAttacks = (m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body)) ? CentipedeBoss.m_settings.m_numBodyAttacksDamaged : CentipedeBoss.m_settings.m_numBodyAttacks;

        // Perform attacks
        for (int i = 0; i < numAttacks; i++)
        {
            float timeBetween = 0.4f; // Time offset needed to fire projectiles down one row
            StartCoroutine(FireBodyProjectiles(timeBetween));
            
            // Wait for projectiles to fire
            yield return new WaitForSeconds(timeBetween * CentipedeBoss.m_settings.m_numBodyProjectiles);

            // After projectiles have fired, have a delay before the next ones fire
            float delay = (m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body)) ? CentipedeBoss.m_settings.m_bodyTimeBetweenFiringDamaged : CentipedeBoss.m_settings.m_bodyTimeBetweenFiring;
            yield return new WaitForSeconds(delay);
        }

        CompleteBehaviour();
    }

    private IEnumerator FireBodyProjectiles(float _timeBetween)
    {
        // Fire projectiles, starting with the front body segment
        for (int i = 0; i < CentipedeBoss.m_settings.m_numBodyProjectiles; i++)
        {
            // Calculate projectile speed
            bool bodyDamaged = m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body);
            float projectileSpeed = (bodyDamaged) ? CentipedeBoss.m_settings.m_bodyProjectileSpeedDamaged : CentipedeBoss.m_settings.m_bodyProjectileSpeed;

            // Fire projectiles
            m_segmentFirers[i].FireProjectiles(projectileSpeed);

            // Activate the section of the body that is firing, for a short time
            m_centipedeHealth.ActivateSection(true, i + 1);
            yield return new WaitForSeconds(_timeBetween);
            StartCoroutine(DeactivateSectionAfter(i + 1, _timeBetween));
        }
    }

    // Deactivate a section of the body after a delay
    private IEnumerator DeactivateSectionAfter(int _sectionIndex, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        m_centipedeHealth.ActivateSection(false, _sectionIndex);
    }

    public override void Reset()
    {
        base.Reset();
    }
}
