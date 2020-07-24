using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class TutorialChunk : MonoBehaviour
{
    [HideInInspector] public UnityEvent m_chunkPunchedEvent = new UnityEvent();
    [HideInInspector] public UnityEvent m_chunkDestroyedEvent = new UnityEvent();

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.chunkHit, ChunkPunched);
        MessageBus.AddListener(EMessageType.chunkDestroyed, ChunkDestroyed);
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.chunkHit, ChunkPunched);
        MessageBus.RemoveListener(EMessageType.chunkDestroyed, ChunkDestroyed);
    }

    // Calls the event when the chunk is punched
    private void ChunkPunched(string _null)
    {
        m_chunkPunchedEvent.Invoke();
    }

    // Calls the event when the chunk is destroyed
    private void ChunkDestroyed(string _null)
    {
        m_chunkDestroyedEvent.Invoke();
    }
}
