using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public bool m_isWalkable;
    public Vector3 m_worldPosition;

    public PathNode(bool _isWalkable, Vector3 _worldPosition)
    {
        m_isWalkable = _isWalkable;
        m_worldPosition = _worldPosition;
    }
}
