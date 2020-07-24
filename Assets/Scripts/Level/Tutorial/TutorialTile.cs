using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class TutorialTile : MonoBehaviour
{
    [SerializeField] private UnityEvent m_whenRaisedEvent = new UnityEvent();
    [SerializeField] private UnityEvent m_whenChunkPunched = new UnityEvent();
    [SerializeField] private UnityEvent m_whenChunkDestroyed = new UnityEvent();

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.chunkRaise, ChunkRaised);
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.chunkRaise, ChunkRaised);
    }

    // Calls the event when a chunk is raised
    private void ChunkRaised(string _null)
    {
        m_whenRaisedEvent.Invoke();

        Chunk chunk = FindObjectOfType<Chunk>();

        // Add script
        TutorialChunk tChunk = chunk.gameObject.AddComponent<TutorialChunk>();

        tChunk.m_chunkPunchedEvent = m_whenChunkPunched;
        tChunk.m_chunkDestroyedEvent = m_whenChunkDestroyed;

        Destroy(this);
    }
}
