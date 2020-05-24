using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public bool m_isWalkable;
    public Vector3 m_worldPosition;
    public int m_gridX;
    public int m_gridY;

    public int m_gCost;
    public int m_hCost;
    public PathNode m_parent;
    public bool m_occupied = false;
    public int m_occupiedFor = 0;

    public PathNode(bool _isWalkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        m_isWalkable = _isWalkable;
        m_worldPosition = _worldPosition;

        m_gridX = _gridX;
        m_gridY = _gridY;

        ResetNode();
    }

    public void ResetNode()
    {
        m_gCost = 0;
    }

    public int FCost
    {
        get
        {
            int fCost = m_gCost + m_hCost;
            // If occupied
            if (m_occupied)
            {
                // If segment won't be gone by the time we get there, raise fCost greatly
                if (m_gCost <= m_occupiedFor) fCost += 1000;
            }
            return fCost;
        }
    }
}
