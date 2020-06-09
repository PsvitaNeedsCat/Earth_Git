using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBodySegment : MonoBehaviour
{
    public CentipedeLaser m_laser;

    public void FireLaserFor(float _duration)
    {
        m_laser.FireLaserFor(_duration);
    }

    public void FireWarning(float _duration)
    {
        m_laser.FireWarning(_duration);
    }
}
