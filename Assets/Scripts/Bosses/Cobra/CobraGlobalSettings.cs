using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cobra Global Settings", menuName = "Settings/CobraGlobalSettings")]
public class CobraGlobalSettings : ScriptableObject
{
    [Header("General Settings")]
    public int m_maxHealth = 3;
    public int m_arenaLength;

    [Header("Mirage Wall Settings")]
    public float m_wallTravelDistance;
    public float m_wallSpawnDistance;

    [Header("Mirage Block Scramble Settings")]
    public float m_timeBeforeGenerate;
    public List<string> m_blockLayouts; // B = Blue block, R = Red block, N = None

    [Header("Chase Settings")]
    public int m_jumpsBeforeDeath;
    public float m_jumpDuration;
    public float m_jumpDelay;
    public float m_bigJumpHeight;
    public float m_bigJumpDuration;

    [Header("Jump Drop Shadow Settings")]
    public float m_dropShadowMinScale = 0.01f;
    public float m_dropShadowMaxScale = 0.1f;
}
