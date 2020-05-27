using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Centipede Settings", menuName = "Settings/CentipedeSettings")]
public class CentipedeSettings : ScriptableObject
{
    public LayerMask m_blocksLasers;
    public float m_laserWidth;
    public float m_laserDistance;
    public float m_laserDuration;
    public float m_timeBetweenLasers;
    public int m_lasersAtOnce;
    public int m_timesLasersFired;
    public float m_moveSpeed;
}