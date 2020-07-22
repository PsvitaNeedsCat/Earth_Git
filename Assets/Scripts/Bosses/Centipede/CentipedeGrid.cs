using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// The grid in which the centipede does its path finding. Holds a 2D array of nodes
public class CentipedeGrid : MonoBehaviour
{
    public float m_nodeRadius;

    // Nodes that make up the grid
    private static PathNode[,] s_grid;

    private float m_nodeDiameter;
    private readonly static int s_gridSize = 25;
    private static Vector2 s_gridWorldSize = Vector2.one * 25.0f;
    private readonly Vector2Int[] m_neighbourDirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    private void Awake()
    {
        m_nodeDiameter = m_nodeRadius * 2.0f;
        CreateGrid();
    }

    // Generates a 2D array of path nodes to create the grid, 
    private void CreateGrid()
    {
        s_grid = new PathNode[s_gridSize, s_gridSize];

        // Find bottom left position of grid in world
        Vector3 worldBottomLeft = transform.position - Vector3.right * s_gridWorldSize.x / 2.0f - Vector3.forward * s_gridWorldSize.y / 2.0f;

        // Create grid
        for (int x = 0; x < s_gridSize; x++)
        {
            for (int y = 0; y < s_gridSize; y++)
            {
                // Find point in world of this node
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) + Vector3.forward * (y * m_nodeDiameter + m_nodeRadius);

                // Add new node
                s_grid[x, y] = new PathNode(true, worldPoint, x, y);
            }
        }

        // Find all obstacles in the scene
        CentipedeObstacle[] obstacles = FindObjectsOfType<CentipedeObstacle>();

        // For every obstacle, make the grid node containing it unwalkable
        foreach (CentipedeObstacle obstacle in obstacles)
        {
            PathNode node = NodeFromWorldPoint(obstacle.transform.position);
            node.m_isWalkable = false;
        }
    }

    // Resets all nodes contained in the grid
    public void ResetNodes()
    {
        for (int x = 0; x < s_gridSize; x++)
        {
            for (int y = 0; y < s_gridSize; y++)
            {
                s_grid[x, y].ResetNode();
            }
        }
    }

    public List<PathNode> GetNeighbours(PathNode _node)
    {
        List<PathNode> neighbours = new List<PathNode>();

        // Check all surrounding nodes in grid
        for (int i = 0; i < m_neighbourDirs.Length; i++)
        {
            Vector2Int neighbourDir = m_neighbourDirs[i];
            int checkX = _node.m_gridX + neighbourDir.x;
            int checkY = _node.m_gridY + neighbourDir.y;

            // Add to list if within bounds
            if (checkX >= 0 && checkX < s_gridSize && checkY >= 0 && checkY < s_gridSize)
            {
                neighbours.Add(s_grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    // Returns the closest grid node to a given world position
    public static PathNode NodeFromWorldPoint(Vector3 _worldPosition)
    {
        // Convert world position to normalised grid coordinates
        float percentX = (_worldPosition.x + s_gridWorldSize.x / 2.0f) / s_gridWorldSize.x;
        float percentY = (_worldPosition.z + s_gridWorldSize.y / 2.0f) / s_gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Convert to grid coordinates
        int x = Mathf.RoundToInt((s_gridSize - 1) * percentX);
        int y = Mathf.RoundToInt((s_gridSize - 1) * percentY);

        return s_grid[x, y];
    }

    // Used for drawing gizmos
    public List<PathNode> m_path;

    private void OnDrawGizmos()
    {
        // Draw a cube around the whole grid
        Gizmos.DrawWireCube(transform.position, new Vector3(s_gridWorldSize.x, 1.0f, s_gridWorldSize.y));

        if (s_grid != null)
        {
            foreach (PathNode n in s_grid)
            {
                Gizmos.color = Color.red;
                if (m_path != null)
                {
                    // Draw points on the path green
                    if (m_path.Contains(n)) Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(n.m_worldPosition, Vector3.one * 0.9f);
                }

#if UNITY_EDITOR
                // Draw the FCost of each node on it
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
                Handles.Label(n.m_worldPosition, n.FCost.ToString());

#endif
            }
        }
    }
}
