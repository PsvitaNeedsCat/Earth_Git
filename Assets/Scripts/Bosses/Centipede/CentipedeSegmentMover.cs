using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeSegmentMover : MonoBehaviour
{
    public int m_positionInBody;
    [HideInInspector] public CentipedeSegmentMover m_segmentBehind;

    private Vector3 m_lastPosition;
    private Quaternion m_lastRotation;
    private Vector3 m_targetPosition;
    private Quaternion m_targetRotation;
    private CentipedeHealth m_centipedeHealth;

    private PathNode m_currentNode;

    private void Start()
    {
        m_lastPosition = transform.position;
        m_lastRotation = transform.rotation;
        m_centipedeHealth = GetComponentInParent<CentipedeHealth>();
    }

    // Setup variables
    public void Init()
    {
        m_currentNode = CentipedeGrid.NodeFromWorldPoint(transform.position);
        m_currentNode.m_occupiedFor = 7 - m_positionInBody;
    }

    // Update last position and rotation variables when position is reached
    public void ReachedPosition()
    {
        m_lastPosition = transform.position;
        m_lastRotation = transform.rotation;
        m_segmentBehind?.ReachedPosition();
    }

    // Moves this segment on to the next target position and rotation
    public void NextPos(Vector3 _newPos, Quaternion _newRot)
    {
        m_currentNode.m_occupied = false;
        m_currentNode.m_occupiedFor = 0;

        m_lastPosition = transform.position;
        m_lastRotation = transform.rotation;
        
        m_targetPosition = _newPos;
        m_targetRotation = _newRot;

        // Update the node we are occupying with information about this segment
        m_currentNode = CentipedeGrid.NodeFromWorldPoint(m_targetPosition);
        m_currentNode.m_occupied = true;
        m_currentNode.m_occupiedFor = 7 - m_positionInBody;

        m_segmentBehind?.NextPos(m_lastPosition, m_lastRotation);
    }

    // Moves a segment to a position inbetween its last and next positions, with a float
    public void Move(float _t)
    {
        transform.position = Vector3.Lerp(m_lastPosition, m_targetPosition, _t);

        // Head tracks its own rotation, target rotation not used
        transform.rotation = Quaternion.Lerp(m_lastRotation, m_targetRotation, _t);

        // Move child
        m_segmentBehind?.Move(_t);
    }

    public void Damaged()
    {
        m_centipedeHealth.DamageSection(m_positionInBody);
    }
}
