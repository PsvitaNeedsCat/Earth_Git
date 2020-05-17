using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeSegment : MonoBehaviour
{
    public bool m_isHead = false;
    [HideInInspector] public CentipedeSegment m_segmentBehind;
    private Vector3 m_lastPosition;
    private Quaternion m_lastRotation;

    private Vector3 m_targetPosition;
    private Quaternion m_targetRotation;

    private void Awake()
    {
        m_lastPosition = transform.position;
        m_lastRotation = transform.rotation;
    }

    // Moves this segment on to the next target position and rotation
    public void NextPos(Vector3  _newPos, Quaternion _newRot)
    {
        m_lastPosition = transform.position;
        m_lastRotation = transform.rotation;
        
        m_targetPosition = _newPos;
        m_targetRotation = _newRot;

        m_segmentBehind?.NextPos(m_lastPosition, m_lastRotation);
    }

    // Moves a segment to a position inbetween its last and next positions, with a float
    public void Move(float _t)
    {
        transform.position = Vector3.Lerp(m_lastPosition, m_targetPosition, _t);

        // Head tracks its own rotation, target rotation not used
        // if (!m_isHead)
        transform.rotation = Quaternion.Lerp(m_lastRotation, m_targetRotation, _t);

        // Move child
        m_segmentBehind?.Move(_t);
    }
}
