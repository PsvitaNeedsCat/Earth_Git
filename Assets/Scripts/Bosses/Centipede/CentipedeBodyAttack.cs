﻿using System.Collections;
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

        CentipedeMovement.m_seekingTarget = true;
        CentipedeMovement.m_loopTargets = true;
        CentipedeMovement.SetTargets(m_movePoints);

        m_centipedeHealth.ActivateSection(true, CentipedeHealth.ESegmentType.body);

        StartCoroutine(FiringSequence());
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();

        CentipedeMovement.m_seekingTarget = false;
        CentipedeMovement.m_loopTargets = false;
    }

    private IEnumerator FiringSequence()
    {
        yield return new WaitForSeconds(CentipedeBoss.m_settings.m_bodyAttackStartDelay);

        for (int i = 0; i < CentipedeBoss.m_settings.m_numBodyAttacks; i++)
        {
            float timeBetween = 0.4f;
            StartCoroutine(FireBodyProjectiles(timeBetween));

            yield return new WaitForSeconds(timeBetween * CentipedeBoss.m_settings.m_numBodyProjectiles);
            yield return new WaitForSeconds(CentipedeBoss.m_settings.m_bodyTimeBetweenFiring);
        }

        m_centipedeHealth.ActivateSection(false, CentipedeHealth.ESegmentType.body);

        CompleteBehaviour();
    }

    private IEnumerator FireBodyProjectiles(float _timeBetween)
    {
        for (int i = 0; i < CentipedeBoss.m_settings.m_numBodyProjectiles; i++)
        {
            bool bodyDamaged = m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.body);
            float projectileSpeed = (bodyDamaged) ? CentipedeBoss.m_settings.m_bodyProjectileSpeedDamaged : CentipedeBoss.m_settings.m_bodyProjectileSpeed;

            m_segmentFirers[i].FireProjectiles(projectileSpeed);

            yield return new WaitForSeconds(_timeBetween);
        }
    }

    public override void Reset()
    {
        base.Reset();
    }
}