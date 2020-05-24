using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeMovement : MonoBehaviour
{
    public List<CentipedeSegmentMover> m_segments = new List<CentipedeSegmentMover>();
    public CentipedeGrid m_grid;
    public CentipedePathfinding m_pathfinder;
    public Transform m_target;

    private float t = 0.0f;
    private Quaternion headTargetRotation;
    private List<PathNode> m_path;
    private int m_positionInPath = 0;

    private void Awake()
    {
        for (int i = 0; i < m_segments.Count - 1; i++)
        {
            m_segments[i].m_segmentBehind = m_segments[i + 1];
        }
    }

    private void Start()
    {
        headTargetRotation = m_segments[0].transform.rotation;
        m_segments[0].NextPos(m_segments[0].transform.position + m_segments[0].transform.forward, m_segments[0].transform.rotation);
        GetPath();
    }

    private void Update()
    {
        t += Time.smoothDeltaTime;
        Step(t);
    }

    private void Step(float _t)
    {
        PathNode head = CentipedeGrid.NodeFromWorldPoint(m_segments[0].transform.position);
        PathNode target = CentipedeGrid.NodeFromWorldPoint(m_target.position);

        if (t >= 1.0f)
        {
            GetPath();

            // If we're at the end of the current path, wait
            if (head.m_gridX == target.m_gridX && head.m_gridY == target.m_gridY) return;
            //// If not at the target, but at the end of the current path, we need a new path
            //else if (m_positionInPath == m_path.Count - 1)
            //{
            //    GetPath();
            //    NextPathPoint(true);
            //}
            //else NextPathPoint(false);

            NextPathPoint(true);

            t -= 1.0f;
            // Debug.Log("Next path point");
            return;
        }

        m_segments[0].Move(_t);
    }

    private void NextPathPoint(bool _first)
    {
        if (m_positionInPath == m_path.Count - 1 && !_first)
        {
            Debug.Log("Can't go to next position in path, at end");
            return;
        }

        if (!_first) m_positionInPath++;

        PathNode nextPoint = m_path[m_positionInPath];
        m_segments[0].NextPos(nextPoint.m_worldPosition, headTargetRotation);
    }

    private void GetPath()
    {
        // Will be incremented before use
        m_positionInPath = 0;
        m_path = m_pathfinder.GetPath(m_segments[0].transform, m_target);
    }

    //private void Forward(float _input)
    //{
    //    t += Time.smoothDeltaTime * 2.0f * _input * CentipedeBoss.GetCurrentStateInfo().m_moveSpeed;

    //    if (t >= 1.0f)
    //    {
    //        m_segments[0].NextPos(m_segments[0].transform.position + m_segments[0].transform.forward, headTargetRotation);
    //        t = 0.0f;
    //    }

    //    m_segments[0].Move(t);
    //}

    //private void Turn(float _horInput)
    //{
    //    headTargetRotation = Quaternion.Euler(Vector3.up * 90.0f * _horInput) * headTargetRotation;
    //}
}
