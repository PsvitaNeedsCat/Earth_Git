using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedePathfinding : MonoBehaviour
{
    private Transform m_seeker;
    private Transform m_target;

    private CentipedeGrid m_grid;
    private List<PathNode> m_path;

    private void Awake()
    {
        m_grid = GetComponent<CentipedeGrid>();
    }

    // Get a path between two transforms
    public List<PathNode> GetPath(Transform _seeker, Transform _target)
    {
        m_seeker = _seeker;
        m_target = _target;

        m_grid.ResetNodes();
        FindPath(m_seeker.position, m_target.position);
        m_grid.m_path = m_path;
        return m_path;
    }

    // Find a path through the grid, between two positions
    private void FindPath(Vector3 _startPosition, Vector3 _targetPosition)
    {
        PathNode startNode = CentipedeGrid.NodeFromWorldPoint(_startPosition);
        PathNode targetNode = CentipedeGrid.NodeFromWorldPoint(_targetPosition);

        List<PathNode> openSet = new List<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        openSet.Add(startNode);

        // Iterate through until solved
        while (openSet.Count > 0)
        {
            // Update current node
            PathNode currentNode = openSet[0];
            
            // Check all other nodes on the open set
            for(int i = 1; i < openSet.Count; i++)
            {
                PathNode checkNode = openSet[i];

                // If the node we are checking has a lower F cost, or equal but has a lower H cost, make it our new current node
                if (checkNode.FCost < currentNode.FCost || (checkNode.FCost == currentNode.FCost && checkNode.m_hCost < currentNode.m_hCost))
                {
                    currentNode = checkNode;
                }
            }

            // Move current node to the closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Found target node!
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            // Check all neighbours
            foreach (PathNode neighbour in m_grid.GetNeighbours(currentNode))
            {
                // Continue if not walkable or already in closed set
                if (!neighbour.m_isWalkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.m_gCost + GetDistance(currentNode, neighbour);

                // If new path to neighbour is shorter or neighbour is not in open set
                if (newMovementCostToNeighbour < neighbour.m_gCost || !openSet.Contains(neighbour))
                {
                    // Update neighbour
                    neighbour.m_gCost = newMovementCostToNeighbour;
                    neighbour.m_hCost = GetDistance(neighbour, targetNode);
                    neighbour.m_parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    // Retrace through node parents in order to find the final path
    private void RetracePath(PathNode _startNode, PathNode _endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = _endNode;

        while (currentNode != _startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.m_parent;
        }

        path.Reverse();
        m_path = path;
    }

    // Can't go diagonally, so distance is just abs(delta X + delta Y)
    int GetDistance(PathNode _nodeA, PathNode _nodeB)
    {
        int distX = Mathf.Abs(_nodeA.m_gridX - _nodeB.m_gridX);
        int distY = Mathf.Abs(_nodeA.m_gridY - _nodeB.m_gridY);

        return (distX + distY);
    }
}
