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
    [Tooltip("How far away from the statue the projectile will spawn")]
    public float m_spitSpawnDist = 1.0f;

    [Header("Tongue Enemy")]
    [Tooltip("How long before the enemy extends its tongue again")]
    public float m_TongueCooldown = 3.0f;
    [Tooltip("How much damage the tongue does")]
    public int m_tongueDamage = 1;

    [Header("Grub")]
    [Tooltip("How long the grub will pause for before moving tile")]
    public float m_grubMaxMoveTime = 2.0f;
    [Tooltip("Howlong it takes the grub to move 1 tile")]
    public float m_grubSpeed = 1.0f;
    [Tooltip("How much damage the projectile does")]
    public int m_grubProjDamage = 1;
    [Tooltip("Speed of grub's projectile")]
    public float m_grubProjSpeed = 0.5f;
    [Tooltip("How big the grub gets when growing")]
    public float m_grubGrowSize = 0.8f;
    [Tooltip("How long the grub is vulnerable for")]
    public float m_grubVulnerableTime = 4.0f;

    [Header("Snake Enemy")]
    [Tooltip("How fast the mirage bullets move in the air")]
    public float m_mirageBulletSpeed = 2.0f;
    [Tooltip("The longer the number, the less often the snake fires")]
    public float m_snakeFireDelay = 1.0f;
    [Tooltip("How far away from the snake does the bullet spawn")]
    public float m_snakeBulletSpawnDist = 1.0f;
}
