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
}
