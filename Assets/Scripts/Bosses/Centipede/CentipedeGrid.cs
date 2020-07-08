using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// The grid in which the centipede does its path finding. Holds a 2D array of nodes
public class CentipedeGrid : MonoBehaviour
{
    public float m_nodeRadius;

    // Nodes that make up the grid
    private static PathNode[,] m_grid;

    private float m_nodeDiameter;
    private readonly static int m_gridSize = 25;
    private static Vector2 m_gridWorldSize = Vector2.one * 25.0f;
    private readonly Vector2Int[] m_neighbourDirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    private void Awake()
    {
        m_nodeDiameter = m_nodeRadius * 2.0f;
        CreateGrid();
    }

    // Generates a 2D array of path nodes to create the grid, 
    private void CreateGrid()
    {
        m_grid = new PathNode[m_gridSize, m_gridSize];

        // Find bottom left position of grid in world
        Vector3 worldBottomLeft = transform.position - Vector3.right * m_gridWorldSize.x / 2.0f - Vector3.forward * m_gridWorldSize.y / 2.0f;

        // Create grid
        for (int x = 0; x < m_gridSize; x++)
        {
            for (int y = 0; y < m_gridSize; y++)
            {
                // Find point in world of this node
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) + Vector3.forward * (y * m_nodeDiameter + m_nodeRadius);

                // Add new node
                m_grid[x, y] = new PathNode(true, worldPoint, x, y);
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
        for (int x = 0; x < m_gridSize; x++)
        {
            for (int y = 0; y < m_gridSize; y++)
            {
                m_grid[x, y].ResetNode();
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
            if (checkX >= 0 && checkX < m_gridSize && checkY >= 0 && checkY < m_gridSize)
            {
                neighbours.Add(m_grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    // Returns the closest grid node to a given world position
    public static PathNode NodeFromWorldPoint(Vector3 _worldPosition)
    {
        // Convert world position to normalised grid coordinates
        float percentX = (_worldPosition.x + m_gridWorldSize.x / 2.0f) / m_gridWorldSize.x;
        float percentY = (_worldPosition.z + m_gridWorldSize.y / 2.0f) / m_gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Convert to grid coordinates
        int x = Mathf.RoundToInt((m_gridSize - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSize - 1) * percentY);

        return m_grid[x, y];
    }

    // Used for drawing gizmos
    public List<PathNode> m_path;

    private void OnDrawGizmos()
    {
        // Draw a cube around the whole grid
        Gizmos.DrawWireCube(transform.position, new Vector3(m_gridWorldSize.x, 1.0f, m_gridWorldSize.y));

        if (m_grid != null)
        {
            foreach (PathNode n in m_grid)
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
