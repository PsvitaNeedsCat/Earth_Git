using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CobraMovementGridTile
{
    public Vector3 m_worldPos;
    public int m_index;

    public CobraMovementGridTile(Vector3 _worldPos, int _index)
    {
        m_worldPos = _worldPos;
        m_index = _index;
    }
}

public class CobraMovementGrid : MonoBehaviour
{
    private static List<CobraMovementGridTile> m_gridTiles;
    private static int m_gridSize = 5;
    private static Vector3 m_topLeftPosition;
    private static float m_maxDistance = 1.3f;

    private void Awake()
    {
        // Debug.Log("Generating grid");
        GenerateGrid();
        m_topLeftPosition = transform.position;
    }

    public static Vector3 WorldPosFromIndex(int _index)
    {
        // Debug.Log("Getting world pos of " + _index + " from " + m_gridTiles);
        return m_gridTiles[_index].m_worldPos;
    }

    public static int IndexFromWorldPos(Vector3 _worldPos)
    {
        CobraMovementGridTile closest = null;
        float closestDist = float.MaxValue;

        _worldPos.y = m_topLeftPosition.y;

        for (int i = 0; i < m_gridTiles.Count; i++)
        {
            CobraMovementGridTile tile = m_gridTiles[i];

            float dist = (tile.m_worldPos - _worldPos).magnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = tile;
            }
        }

        if (closest != null && closestDist < m_maxDistance)
        {
            return closest.m_index;
        }
        else
        {
            return -1;
        }
    }

    private static void GenerateGrid()
    {
        // Debug.Log("Creating grid list");
        m_gridTiles = new List<CobraMovementGridTile>();

        for (int i = 0; i < m_gridSize; i++)
        {
            for (int j = 0; j < m_gridSize; j++)
            {
                m_gridTiles.Add(new CobraMovementGridTile(m_topLeftPosition + -Vector3.forward * i + Vector3.right * j, i * m_gridSize + j));
            }
        }

        // Debug.Log("Finished creating grid");
    }

    private void OnDrawGizmosSelected()
    {
        if (m_gridTiles == null)
        {
            return;
        }

        foreach (CobraMovementGridTile tile in m_gridTiles)
        {
            Gizmos.DrawWireCube(tile.m_worldPos, Vector3.one * 0.2f);

#if UNITY_EDITOR
            Handles.Label(tile.m_worldPos + Vector3.up * 0.2f, tile.m_index.ToString());
#endif
        }
    }
}
