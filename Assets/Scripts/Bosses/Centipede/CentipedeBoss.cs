using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBoss : MonoBehaviour
{
    public List<CentipedeBodySegment> m_bodySegments;
    public float m_laserDuration = 5.0f;

    private static int m_segmentsAlive;
    public static CentipedeSettings m_settings;
    private static List<CentipedeStateInfo> m_stateInfo;

    private void Awake()
    {
        m_segmentsAlive = 5;
        m_settings = Resources.Load<CentipedeSettings>("ScriptableObjects/CentipedeBossSettings");
        m_stateInfo = new List<CentipedeStateInfo>();
        m_stateInfo.Add(m_settings.m_oneAlive);
        m_stateInfo.Add(m_settings.m_twoAlive);
        m_stateInfo.Add(m_settings.m_threeAlive);
        m_stateInfo.Add(m_settings.m_fourAlive);
        m_stateInfo.Add(m_settings.m_fiveAlive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // FireRandomLaser();
            m_bodySegments[0].FireLaserFor(m_laserDuration);
            m_bodySegments[1].FireLaserFor(m_laserDuration);
            m_bodySegments[2].FireLaserFor(m_laserDuration);
            m_bodySegments[3].FireLaserFor(m_laserDuration);
            m_bodySegments[4].FireLaserFor(m_laserDuration);
        }
    }

    private void FireRandomLaser()
    {
        int laserIndex = Random.Range(0, 5);
        m_bodySegments[laserIndex].FireLaserFor(m_laserDuration);
    }

    public static CentipedeStateInfo GetCurrentStateInfo()
    {
        return m_stateInfo[m_segmentsAlive - 1];
    }

    public static int SegmentsAlive() { return m_segmentsAlive; }

    public static void SegmentDied()
    {
        m_segmentsAlive--;
        Debug.Log("Segment died, " + m_segmentsAlive + " left");
    }
}
