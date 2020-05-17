using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeGrid : MonoBehaviour
{
    public LayerMask m_unwalkableMask;
    public Vector2 m_gridWorldSize;
    public float m_nodeRadius;

    PathNode[,] m_grid;
    readonly int m_gridSize = 9;
    private float m_nodeDiameter;

    private void Start()
    {
        m_nodeDiameter = m_nodeRadius * 2.0f;
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
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_gridWorldSize.x, 1.0f, m_gridWorldSize.y));
    }
}
