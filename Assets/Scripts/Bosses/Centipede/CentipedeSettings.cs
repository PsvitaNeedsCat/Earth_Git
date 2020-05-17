using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CentipedeStateInfo
{
    public float m_laserWidth;
    public float m_laserDistance;
    public int m_numLasers;
    public float m_moveSpeed;
}

[CreateAssetMenu(fileName = "New Centipede Settings", menuName = "Settings/CentipedeSettings")]
public class CentipedeSettings : ScriptableObject
{
    public CentipedeStateInfo m_fiveAlive;
    public CentipedeStateInfo m_fourAlive;
    public CentipedeStateInfo m_threeAlive;
    public CentipedeStateInfo m_twoAlive;
    public CentipedeStateInfo m_oneAlive;
}