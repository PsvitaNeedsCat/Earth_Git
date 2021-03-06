﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Centipede Settings", menuName = "Settings/CentipedeSettings")]
public class CentipedeSettings : ScriptableObject
{
    [Header("General")]
    public float m_moveSpeed;

    [Header("Laser Settings")]
    public LayerMask m_blocksLasers;
    public float m_laserWidth;
    public float m_laserDistance;
    public float m_laserAnticipation;
    public float m_laserDuration;
    public float m_laserDurationDamaged;
    public float m_timeBetweenLasers;
    public float m_timeBetweenLasersDamaged;
    public int m_lasersAtOnce;
    public int m_lasersAtOnceDamaged;
    public int m_timesLasersFired;

    [Header("Body Attack Settings")]
    public int m_numBodyProjectiles;
    public int m_numBodyAttacks;
    public int m_numBodyAttacksDamaged;
    public float m_bodyProjectileSpeed;
    public float m_bodyProjectileSpeedDamaged;
    public float m_bodyTimeBetweenFiring; // Delay between projectile groups
    public float m_bodyTimeBetweenFiringDamaged;
    public float m_bodyAttackStartDelay; // Delay at start of attack
    public float m_heightOffset;

    [Header("Train Settings")]
    public float m_trainMoveSpeed;
    public float m_trainDamagedMoveSpeed;
    public float m_lavaLifetime;
    public int m_chunksToStun;
    public float m_stunnedFor;

    [Header("Tail Settings")]
    public float m_fireDelay;
    public float m_fireDelayDamaged;
    public float m_firingDuration;
    public float m_projectileSpeed;
    public float m_projectileSpeedDamaged;
    public float m_rotationSpeed;
    public float m_burrowDuration;
    public float m_projectileLifetime;
}