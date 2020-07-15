using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CentipedeMovement : MonoBehaviour
{
    public List<CentipedeSegmentMover> m_segments = new List<CentipedeSegmentMover>();
    public CentipedeGrid m_grid;
    public CentipedePathfinding m_pathfinder;
    public CentipedeAnimations m_animations;

    public static List<Transform> m_targets;
    public static bool m_seekingTarget = false;
    public static bool m_atTarget = false;
    public static bool m_loopTargets = false;
    public static bool m_useTrainSpeed = false;
    public static bool m_burrowed = false;
    public static bool m_burrowing = false;

    private float m_t = 0.0f;
    private static List<PathNode> m_path;
    private static int m_positionInPath = 0;
    private static Transform m_currentTarget;
    private static int m_currentTargetIndex = 0;
    private GameObject m_lavaTrailPrefab;
    private CentipedeHealth m_centipedeHealth;

    private static CentipedeMovement m_instance;

    // Sets the list of targets for the centipede to pathfind to
    public static void SetTargets(List<Transform> _newTargets)
    {
        m_atTarget = false;
        m_targets = _newTargets;
        m_currentTargetIndex = 0;
        m_currentTarget = m_targets[0];

        // After setting new targets, find a path to the current target
        GetPath();
    }

    private void Awake()
    {
        // Single instance
        if (m_instance) { Destroy(m_instance); }
        m_instance = this;

        // Populate references to segments behind
        for (int i = 0; i < m_segments.Count - 1; i++)
        {
            m_segments[i].m_segmentBehind = m_segments[i + 1];
        }

        // Initialise static variables
        m_positionInPath = 0;
        m_currentTargetIndex = 0;
        m_path?.Clear();
        m_currentTarget = null;
        m_targets?.Clear();
        m_seekingTarget = false;
        m_atTarget = false;
        m_loopTargets = false;
        m_useTrainSpeed = false;
        m_burrowed = false;
        m_burrowing = false;

        m_lavaTrailPrefab = Resources.Load<GameObject>("Prefabs/Bosses/Centipede/CentipedeLavaTrail");
        m_centipedeHealth = GetComponent<CentipedeHealth>();
    }

    private void Start()
    {
        // Initialise each segment
        foreach (CentipedeSegmentMover segment in m_segments)
        {
            segment.Init();
        }

        // Tell segments to setup the next target position
        m_segments[0].NextPos(m_segments[0].transform.position + m_segments[0].transform.forward, m_segments[0].transform.rotation);
    }

    private void Update()
    {
        // If normal pathfinding
        if (m_seekingTarget)
        {
            // Determine move speed
            float moveSpeed = CentipedeBoss.m_settings.m_moveSpeed;
            if (m_useTrainSpeed)
            {
                moveSpeed = (m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.head) ? CentipedeBoss.m_settings.m_trainDamagedMoveSpeed : CentipedeBoss.m_settings.m_trainMoveSpeed);
            }

            // Set animation speed based on move speed
            m_animations.SetAnimSpeed(moveSpeed);

            // Increment time
            m_t += Time.deltaTime * moveSpeed;

            // Step astar
            AStarStep(m_t);
        }
        // If burrowing
        else if (m_burrowing)
        {
            // Increment time
            m_t += (Time.smoothDeltaTime / CentipedeBoss.m_settings.m_burrowDuration);

            // If ready to go to next point
            if (m_t > CentipedeBoss.m_settings.m_burrowDuration)
            {
                // If we're not at the end of the points list
                if (m_currentTargetIndex < m_targets.Count - 1)
                {
                    m_currentTargetIndex++;
                    m_currentTarget = m_targets[m_currentTargetIndex];
                    m_t = 0.0f;
                    m_segments[0].NextPos(m_currentTarget.position, m_currentTarget.rotation);
                }
                // If we are, finish burrowing
                else
                {
                    m_burrowing = false;
                    return;
                }
            }

            // Move segments
            m_segments[0].Move(Mathf.Clamp01(m_t));
        }
    }

    // Start burrowing down
    public static void BurrowDown(List<Transform> _points)
    {
        m_instance.m_segments[0].ReachedPosition();
        m_instance.m_t = 0.0f;
        m_burrowing = true;
        m_targets = _points;
        m_currentTargetIndex = 0;
    }

    // Start burrowing up
    public static void BurrowUp(List<Transform> _points)
    {
        m_instance.m_t = 0.0f;
        m_burrowing = true;
        m_targets = _points;
        m_currentTargetIndex = 0;
    }

    // 
    private void AStarStep(float _t)
    {
        PathNode head = CentipedeGrid.NodeFromWorldPoint(m_segments[0].transform.position);
        PathNode target = CentipedeGrid.NodeFromWorldPoint(m_currentTarget.position);

        if (m_t >= 0.99f)
        {
            // If we're at the end of the current path, switch to next target
            if (head.m_gridX == target.m_gridX && head.m_gridY == target.m_gridY)
            {
                m_atTarget = NextTarget();
                
                // return;
            }
            else
            {
                // GetPath();
            }

            // If we're not at the target, get a path to the target
            if (m_atTarget) return;
            else GetPath();

            // Move to next path point
            NextPathPoint(true);

            // If charging and the head is not damaged, drop lava
            if (CentipedeBoss.m_dropLava && !m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.head)) DropLavaTrail();

            m_t -= 1.0f;
            return;
        }

        m_segments[0].Move(_t);
    }

    // Spawn a lava trail under the head
    private void DropLavaTrail()
    {
        Vector3 spawnPos = m_segments[0].transform.position - 0.99f * Vector3.up;
        spawnPos.x = Mathf.Floor(spawnPos.x + 0.5f);
        spawnPos.z = Mathf.Floor(spawnPos.z + 0.5f);
        GameObject lava = Instantiate(m_lavaTrailPrefab, spawnPos, Quaternion.identity, transform.parent);
    }

    // Target the next point in the path
    private void NextPathPoint(bool _first)
    {
        // Check if at the end of the path
        if (m_positionInPath == m_path.Count - 1 && !_first)
        {
            Debug.Log("Can't go to next position in path, at end");
            return;
        }

        if (!_first) m_positionInPath++;

        // Setup segments to target the next point in the path
        PathNode nextPoint = m_path[m_positionInPath];
        Quaternion newHeadRot = Quaternion.LookRotation(nextPoint.m_worldPosition - m_segments[0].transform.position);
        m_segments[0].NextPos(nextPoint.m_worldPosition, newHeadRot);
    }

    // Returns true if we have reached the final target, 
    private bool NextTarget()
    {
        if (!m_loopTargets && m_currentTargetIndex >= m_targets.Count - 1) return true;

        // Increment target index, looping if the variable is set
        if (m_loopTargets)
        {
            m_currentTargetIndex = (m_currentTargetIndex + 1) % m_targets.Count;
        }
        else
        {
            m_currentTargetIndex++;
        }
        
        // Update current target and get a new path to it
        m_currentTarget = m_targets[m_currentTargetIndex];
        GetPath();

        return false;
    }

    private static void GetPath()
    {
        // Will be incremented before use
        m_positionInPath = 0;
        m_path = m_instance.m_pathfinder.GetPath(m_instance.m_segments[0].transform, m_currentTarget);
    }
}
