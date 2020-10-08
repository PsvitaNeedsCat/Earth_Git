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
        CentipedeMovement.s_seekingTarget = true;
        CentipedeMovement.s_loopTargets = true;
        CentipedeMovement.SetTargets(m_movePoints);

        // Start firing projectiles
        StartCoroutine(FiringSequence());
    }

    public override void CompleteBehaviour()
    {
        MessageBus.TriggerEvent(EMessageType.vulnerableEnd);

        base.CompleteBehaviour();

        // Stop pathfinding
        CentipedeMovement.s_seekingTarget = false;
        CentipedeMovement.s_loopTargets = false;
    }

    private IEnumerator FiringSequence()
    {
        // Delay before firing first attack
        yield return new WaitForSeconds(CentipedeBoss.s_settings.m_bodyAttackStartDelay);

        // Determine how many times to fire a line of projectiles
        int numAttacks = (m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body)) ? CentipedeBoss.s_settings.m_numBodyAttacksDamaged : CentipedeBoss.s_settings.m_numBodyAttacks;

        MessageBus.TriggerEvent(EMessageType.vulnerableStart);

        // Perform attacks
        for (int i = 0; i < numAttacks; i++)
        {
            while (!AtBlockCenter(m_segmentFirers[0].transform.position))
            {
                yield return null;
            }

            float timeBetween = 0.4f; // Time offset needed to fire projectiles down one row
            StartCoroutine(FireBodyProjectiles(timeBetween));
            
            // Wait for projectiles to fire
            yield return new WaitForSeconds(timeBetween * CentipedeBoss.s_settings.m_numBodyProjectiles);

            // After projectiles have fired, have a delay before the next ones fire
            float delay = (m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body)) ? CentipedeBoss.s_settings.m_bodyTimeBetweenFiringDamaged : CentipedeBoss.s_settings.m_bodyTimeBetweenFiring;
            yield return new WaitForSeconds(delay);
        }

        CompleteBehaviour();
    }

    private bool AtBlockCenter(Vector3 _position)
    {
        const float lenience = 0.1f;

        float xRemainder = Mathf.Abs(_position.x) % 1.0f;
        float zRemainder = Mathf.Abs(_position.z) % 1.0f;

        return (xRemainder + zRemainder) < lenience;
    }

    private IEnumerator FireBodyProjectiles(float _timeBetween)
    {
        // Fire projectiles, starting with the front body segment
        for (int i = 0; i < CentipedeBoss.s_settings.m_numBodyProjectiles; i++)
        {
            // Calculate projectile speed
            bool bodyDamaged = m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body);
            float projectileSpeed = (bodyDamaged) ? CentipedeBoss.s_settings.m_bodyProjectileSpeedDamaged : CentipedeBoss.s_settings.m_bodyProjectileSpeed;

            // Fire projectiles
            m_segmentFirers[i].FireProjectiles(projectileSpeed, m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body));

            // Activate the section of the body that is firing, for a short time
            m_centipedeHealth.ActivateSection(true, i + 1);
            yield return new WaitForSeconds(_timeBetween);
            StartCoroutine(DeactivateSectionAfter(i + 1, _timeBetween * 3.0f));
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

    public void DisableFireParticles()
    {
        for (int i = 0; i < m_segmentFirers.Count; i++)
        {
            m_segmentFirers[i].m_fireEffects.Stop();
        }
    }
}
