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

    public static List<Transform> s_targets;
    public static bool s_seekingTarget = false;
    public static bool s_atTarget = false;
    public static bool s_loopTargets = false;
    public static bool s_useTrainSpeed = false;
    public static bool s_burrowed = false;
    public static bool s_burrowing = false;

    private float m_t = 0.0f;
    private static List<PathNode> s_path;
    private static int s_positionInPath = 0;
    private static Transform s_currentTarget;
    private static int s_currentTargetIndex = 0;
    private GameObject m_lavaTrailPrefab;
    private CentipedeHealth m_centipedeHealth;

    private static CentipedeMovement s_instance;

    // Sets the list of targets for the centipede to pathfind to
    public static void SetTargets(List<Transform> _newTargets)
    {
        s_atTarget = false;
        s_targets = _newTargets;
        s_currentTargetIndex = 0;
        s_currentTarget = s_targets[0];

        // After setting new targets, find a path to the current target
        GetPath();
    }

    private void Awake()
    {
        // Single instance
        if (s_instance) { Destroy(s_instance); }
        s_instance = this;

        // Populate references to segments behind
        for (int i = 0; i < m_segments.Count - 1; i++)
        {
            m_segments[i].m_segmentBehind = m_segments[i + 1];
        }

        // Initialise static variables
        s_positionInPath = 0;
        s_currentTargetIndex = 0;
        s_path?.Clear();
        s_currentTarget = null;
        s_targets?.Clear();
        s_seekingTarget = false;
        s_atTarget = false;
        s_loopTargets = false;
        s_useTrainSpeed = false;
        s_burrowed = false;
        s_burrowing = false;

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
        if (s_seekingTarget)
        {
            // Determine move speed
            float moveSpeed = CentipedeBoss.s_settings.m_moveSpeed;
            if (s_useTrainSpeed)
            {
                moveSpeed = (m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.head) ? CentipedeBoss.s_settings.m_trainDamagedMoveSpeed : CentipedeBoss.s_settings.m_trainMoveSpeed);
            }

            // Set animation speed based on move speed
            m_animations.SetAnimSpeed(moveSpeed);

            // Increment time
            m_t += Time.deltaTime * moveSpeed;

            // Step astar
            AStarStep(m_t);
        }
        // If burrowing
        else if (s_burrowing)
        {
            // Increment time
            m_t += (Time.smoothDeltaTime / CentipedeBoss.s_settings.m_burrowDuration);

            // If ready to go to next point
            if (m_t > CentipedeBoss.s_settings.m_burrowDuration)
            {
                // If we're not at the end of the points list
                if (s_currentTargetIndex < s_targets.Count - 1)
                {
                    s_currentTargetIndex++;
                    s_currentTarget = s_targets[s_currentTargetIndex];
                    m_t = 0.0f;
                    m_segments[0].NextPos(s_currentTarget.position, s_currentTarget.rotation);
                }
                // If we are, finish burrowing
                else
                {
                    s_burrowing = false;
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
        s_instance.m_segments[0].ReachedPosition();
        s_instance.m_t = 0.0f;
        s_burrowing = true;
        s_targets = _points;
        s_currentTargetIndex = 0;
    }

    // Start burrowing up
    public static void BurrowUp(List<Transform> _points)
    {
        s_instance.m_t = 0.0f;
        s_burrowing = true;
        s_targets = _points;
        s_currentTargetIndex = 0;
    }

    // 
    private void AStarStep(float _t)
    {
        PathNode head = CentipedeGrid.NodeFromWorldPoint(m_segments[0].transform.position);
        PathNode target = CentipedeGrid.NodeFromWorldPoint(s_currentTarget.position);

        if (m_t >= 0.99f)
        {
            // If we're at the end of the current path, switch to next target
            if (head.m_gridX == target.m_gridX && head.m_gridY == target.m_gridY)
            {
                s_atTarget = NextTarget();
                
                // return;
            }
            else
            {
                // GetPath();
            }

            // If we're not at the target, get a path to the target
            if (s_atTarget) return;
            else GetPath();

            // Move to next path point
            NextPathPoint(true);

            // If charging and the head is not damaged, drop lava
            if (CentipedeBoss.s_dropLava && !m_centipedeHealth.IsSectionDamaged(CentipedeHealth.ESegmentType.head)) DropLavaTrail();

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
        if (s_positionInPath == s_path.Count - 1 && !_first)
        {
            Debug.Log("Can't go to next position in path, at end");
            return;
        }

        if (!_first) s_positionInPath++;

        // Setup segments to target the next point in the path
        PathNode nextPoint = s_path[s_positionInPath];
        Quaternion newHeadRot = Quaternion.LookRotation(nextPoint.m_worldPosition - m_segments[0].transform.position);
        m_segments[0].NextPos(nextPoint.m_worldPosition, newHeadRot);
    }

    // Returns true if we have reached the final target, 
    private bool NextTarget()
    {
        if (!s_loopTargets && s_currentTargetIndex >= s_targets.Count - 1) return true;

        // Increment target index, looping if the variable is set
        if (s_loopTargets)
        {
            s_currentTargetIndex = (s_currentTargetIndex + 1) % s_targets.Count;
        }
        else
        {
            s_currentTargetIndex++;
        }
        
        // Update current target and get a new path to it
        s_currentTarget = s_targets[s_currentTargetIndex];
        GetPath();

        return false;
    }

    private static void GetPath()
    {
        // Will be incremented before use
        s_positionInPath = 0;
        s_path = s_instance.m_pathfinder.GetPath(s_instance.m_segments[0].transform, s_currentTarget);
    }
}
