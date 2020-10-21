using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;

public class EpilogueChunk : MonoBehaviour
{
    [HideInInspector] public UnityEvent m_completedMoveEvent = new UnityEvent();

    private bool m_tweening = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Hurtbox>())
        {
            MoveForward();

            MessageBus.TriggerEvent(EMessageType.chunkHit);
        }
    }

    // Tweens forward one block's length
    private void MoveForward()
    {
        if (m_tweening)
        {
            return;
        }

        m_tweening = true;

        float newZ = transform.parent.position.z + 1.0f;
        transform.parent.DOMoveZ(newZ, 0.5f).OnComplete(() => CompletedMove());
    }

    // Invokes the completed move event
    private void CompletedMove()
    {
        m_completedMoveEvent.Invoke();

        m_tweening = false;
    }
}
