using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CentipedeGrid : MonoBehaviour
{
    public float m_nodeRadius;

    static PathNode[,] m_grid;
    readonly static int m_gridSize = 18;
    private float m_nodeDiameter;

    private static Vector2 m_gridWorldSize = Vector2.one * 18.0f;
    private readonly Vector2Int[] m_neighbourDirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    private void Awake()
    {
        m_nodeDiameter = m_nodeRadius * 2.0f;
        CreateGrid();
    }

    private void CreateGrid()
    {
        m_grid = new PathNode[m_gridSize, m_gridSize];
        // Find bottom position of grid in world
        Vector3 worldBottomLeft = transform.position - Vector3.right * m_gridWorldSize.x / 2.0f - Vector3.forward * m_gridWorldSize.y / 2.0f;

        for (int x = 0; x < m_gridSize; x++)
        {
            for (int y = 0; y < m_gridSize; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) + Vector3.forward * (y * m_nodeDiameter + m_nodeRadius);
                m_grid[x, y] = new PathNode(true, worldPoint, x, y);
            }
        }
    }

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

            // Add to list if in bounds
            if (checkX >= 0 && checkX < m_gridSize && checkY >= 0 && checkY < m_gridSize)
            {
                neighbours.Add(m_grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    public static PathNode NodeFromWorldPoint(Vector3 _worldPosition)
    {
        // Convert world position to normalised grid coordinates
        float percentX = (_worldPosition.x + m_gridWorldSize.x / 2.0f) / m_gridWorldSize.x;
        float percentY = (_worldPosition.z + m_gridWorldSize.y / 2.0f) / m_gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((m_gridSize - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSize - 1) * percentY);
        return m_grid[x, y];
    }

    public List<PathNode> m_path;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_gridWorldSize.x, 1.0f, m_gridWorldSize.y));

        if (m_grid != null)
        {
            foreach (PathNode n in m_grid)
            {
                Gizmos.color = Color.red;
                if (m_path != null)
                {
                    // if (n.m_occupied) Gizmos.color = Color.cyan;
                    // if (m_path.Contains(n)) Gizmos.color = Color.green;
                    //Gizmos.color = Color.blue;
                    //if (n.FCost < 20) Gizmos.color = Color.green;
                    //if (n.FCost > 1000) Gizmos.color = Color.cyan;
                }

#if UNITY_EDITOR
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
                Handles.Label(n.m_worldPosition, n.FCost.ToString());

#endif

                Gizmos.DrawWireCube(n.m_worldPosition, Vector3.one * (m_nodeDiameter - 0.1f));
            }
        }
    }
}
