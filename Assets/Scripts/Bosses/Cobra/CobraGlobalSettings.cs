using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraGlobalSettings : ScriptableObject
{
    [Header("General Settings")]
    public int m_maxHealth = 3;

    [Header("Mirage Wall Settings")]
    public float m_wallTravelDistance;
    
}
