using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlobalEnemySettings", menuName = "Settings/GlobalEnemySettings")]
public class GlobalEnemySettings : ScriptableObject
{
    [Header("Spitting Enemy")]
    [Tooltip("How much damage the spit does")]
    public int m_spitDamage = 1;
    [Tooltip("How long before the enemy will spit again")]
    public float m_spitCooldown = 2.0f;
    [Tooltip("Speed of the spit")]
    public float m_spitProjectileSpeed = 1.0f;

    [Header("Tongue Enemy")]
    [Tooltip("How long before the enemy extends its tongue again")]
    public float m_TongueCooldown = 3.0f;
    [Tooltip("How much damage the tongue does")]
    public int m_tongueDamage = 1;

    [Header("Grub")]
    [Tooltip("How often the grub will move a bit")]
    public float m_grubMaxMoveTime = 2.0f;
    [Tooltip("How much force the grub puts into moving")]
    public float m_grubMoveForce = 10.0f;
    [Tooltip("How much damage the projectile does")]
    public int m_grubProjDamage = 1;
    [Tooltip("Speed of grub's projectile")]
    public float m_grubProjSpeed = 0.5f;
}
