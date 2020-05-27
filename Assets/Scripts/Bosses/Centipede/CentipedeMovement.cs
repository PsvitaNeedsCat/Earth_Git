using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeMovement : MonoBehaviour
{
    public List<CentipedeSegmentMover> m_segments = new List<CentipedeSegmentMover>();
    public CentipedeGrid m_grid;
    public CentipedePathfinding m_pathfinder;
    public Transform[] m_targets;
    public static bool m_seekingTarget = false;

    private float t = 0.0f;
    private List<PathNode> m_path;
    private int m_positionInPath = 0;
    private Transform m_currentTarget;
    private int m_currentTargetIndex = 0;
    

    private void Awake()
    {
        for (int i = 0; i < m_segments.Count - 1; i++)
        {
            m_segments[i].m_segmentBehind = m_segments[i + 1];
        }

        m_seekingTarget = false;
        m_currentTarget = m_targets[0];
    }

    private void Start()
    {
        m_segments[0].NextPos(m_segments[0].transform.position + m_segments[0].transform.forward, m_segments[0].transform.rotation);
        GetPath();
    }

    private void Update()
    {
        if (!m_seekingTarget) return;

        t += Time.smoothDeltaTime * CentipedeBoss.m_settings.m_moveSpeed;
        Step(t);
    }

    private void Step(float _t)
    {
        PathNode head = CentipedeGrid.NodeFromWorldPoint(m_segments[0].transform.position);
        PathNode target = CentipedeGrid.NodeFromWorldPoint(m_currentTarget.position);

        if (t >= 1.0f)
        {
            GetPath();

            // If we're at the end of the current path, switch to next target
            if (head.m_gridX == target.m_gridX && head.m_gridY == target.m_gridY)
            {
                NextTarget();
                return;
            }

            NextPathPoint(true);

            t -= 1.0f;
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
        Quaternion newHeadRot = Quaternion.LookRotation(nextPoint.m_worldPosition - m_segments[0].transform.position);
        m_segments[0].NextPos(nextPoint.m_worldPosition, newHeadRot);
    }

    private void NextTarget()
    {
        m_currentTargetIndex = (m_currentTargetIndex + 1) % m_targets.Length;
        m_currentTarget = m_targets[m_currentTargetIndex];
        GetPath();
    }

    private void GetPath()
    {
        // Will be incremented before use
        m_positionInPath = 0;
        m_path = m_pathfinder.GetPath(m_segments[0].transform, m_currentTarget);
    }
}
