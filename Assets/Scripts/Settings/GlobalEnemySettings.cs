using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlobalEnemySettings", menuName = "Settings/GlobalEnemySettings")]
public class GlobalEnemySettings : ScriptableObject
{
    [Header("Spitting Enemy")]
    public GameObject m_spitPrefab;
    public int m_spitDamage = 1;
    public float m_spitCooldown = 2.0f;
    public float m_spitProjectileSpeed = 1.0f;

    [Header("Tongue Enemy")]
    public float m_TongueCooldown = 3.0f;
    public int m_tongueDamage = 1;
}
