using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class EpilogueLazer : MonoBehaviour
{
    [SerializeField] private UnityEvent m_statueHitEvent = new UnityEvent();

    private int m_punchedCount = 4; // Check after punch

    // Called by EpilogueChunk - counts down the punches until it reaches the statue
    public void ChunkPunched()
    {
        --m_punchedCount;

        if (m_punchedCount <= 0)
        {
            m_statueHitEvent.Invoke();
            Destroy(gameObject);
        }
        else
        {
            // Move forward (toward statue)
            Vector3 newPos = transform.position;
            newPos.z += 1.0f;
            transform.position = newPos;
        }
    }
}
