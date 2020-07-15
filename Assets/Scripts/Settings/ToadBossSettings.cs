using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewToadBossSettings", menuName = "Settings/ToadBossSettings")]
public class ToadBossSettings : ScriptableObject
{
    public int m_maxHealth = 3;

    public float m_wakeAfter = 5.0f;

    public float m_swellUpOver = 1.0f;
    public float m_staySwelledUpFor = 5.0f;

    public float m_underwaterTime = 5.0f;

    [Tooltip("0 degrees is directly up")]
    public float m_fragmentAngle = 20.0f;

    public float m_fragmentForce = 2.0f;
}
