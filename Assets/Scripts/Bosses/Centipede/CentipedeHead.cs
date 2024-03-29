﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeHead : MonoBehaviour
{
    public BoxCollider m_damageTrigger;
    private CentipedeTrainAttack m_trainAttack;
    [HideInInspector] public CentipedeHealth m_health;

    private void Awake()
    {
        m_trainAttack = GetComponentInParent<CentipedeTrainAttack>();
        m_damageTrigger = GetComponent<BoxCollider>();
        m_health = m_trainAttack.gameObject.GetComponent<CentipedeHealth>();
    }

    public void DisableCollider()
    {
        m_damageTrigger.enabled = false;
    }
    public void EnableCollider()
    {
        m_damageTrigger.enabled = true;
    }

    // Damage things when hit by the centipede's head
    private void OnTriggerEnter(Collider other)
    {
        if (CentipedeTrainAttack.s_stunned)
        {
            return;
        }

        // Damage the player if they're hit by the centipede head
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            // Calculate knockback on the player
            Vector3 dir = (player.transform.position - transform.position);
            dir.y = 0.0f;
            player.KnockBack(dir.normalized);
            player.GetComponent<HealthComponent>().Health -= 1;

            return;
        }

        // During the train attack, destroy any chunks hit
        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk && !other.isTrigger)
        {
            chunk.GetComponent<HealthComponent>().Health = 0;

            if (CentipedeTrainAttack.s_charging && !CentipedeTrainAttack.s_stunned && !m_health.IsSectionDamaged(CentipedeHealth.ESegmentType.head))
            {
                m_trainAttack.HitByChunk();
            }
        }
    }
}
