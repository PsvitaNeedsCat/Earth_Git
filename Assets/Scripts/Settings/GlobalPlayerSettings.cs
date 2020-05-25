using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlobalPlayerSettings", menuName = "Settings/GlobalPlayerSettings")]
public class GlobalPlayerSettings : ScriptableObject
{
    [Header("Cooldowns")]
    [Tooltip("How long before the player can punch again")]
    public float m_punchCooldown = 1.0f;
    [Tooltip("How long before the player can attempt to raise a chunk again")]
    public float m_raiseCooldown = 1.0f;

    [Header("Physics")]
    [Tooltip("How much gravity is acting on the player")]
    public float m_gravity = -300.0f;
    [Tooltip("Amount of drag on player's X and Y")]
    public float m_drag = 2.0f;
    [Tooltip("How much force is applied to the player when moving")]
    public float m_moveForce = 10.0f;
    [Tooltip("How much force is applied while walking through sand")]
    public float m_sandMoveForce = 5.0f;

    [Header("Tile Targeter")]
    [Tooltip("Maximum range between tile targeter position and closest tile")]
    public float m_maxTileRange = 5.0f;
    [Tooltip("Minimum range between query position and tile")]
    public float m_minTileRange = 7.0f;

    [Tooltip("Maximum number of chunks allowed at once")]
    public int m_maxChunks = 3;

    [Header("Health")]
    [Tooltip("Maximum health player is allowed without upgrades")]
    public int m_defaultMaxHealth = 3;
    [Tooltip("Colour the player flashes when hurt")]
    public Color m_hurtColour = Color.red;
    [Tooltip("How long the player is invincible for when hurt")]
    public float m_hurtTime = 2.0f;

    [Header("Hurtbox Settings")]
    [Tooltip("The amount of frames the hurtbox will exist before being destroyed")]
    public int m_framesBeforeDestroy = 10;
    [Tooltip("The amount of force chunks are hit with")]
    public float m_chunkHitForce = 10000.0f;
    [Tooltip("How much horiztonally to move the hurt box by when spawning")]
    public float m_hurtboxMoveBy = 5.0f;

    [Space(10.0f)]

    [Tooltip("How much knockback is applied to player when the function is called")]
    public float m_knockbackForce = 35.0f;
}
