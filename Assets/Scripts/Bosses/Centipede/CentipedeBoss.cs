using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBoss : MonoBehaviour
{
    public List<CentipedeBodySegment> m_bodySegments;

    private static int m_segmentsAlive;
    private CentipedeSettings m_settings;
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
            FireRandomLaser();
        }
    }

    private void FireRandomLaser()
    {
        int laserIndex = Random.Range(0, 5);
        m_bodySegments[laserIndex].FireLaser();
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
