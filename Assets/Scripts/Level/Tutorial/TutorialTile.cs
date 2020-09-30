using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class TutorialTile : MonoBehaviour
{
    [SerializeField] private bool m_enabled = true;

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
        if (!m_enabled)
        {
            return;
        }

        // Check that this was the correct tile
        Vector3 centre = transform.position;
        centre.y += 0.5f;
        Collider[] colliders = Physics.OverlapBox(centre, Vector3.one * 0.45f, Quaternion.identity);
        if (colliders.Length <= 0)
        {
            return;
        }
        Chunk chunk = null;
        for (int i = 0; i < colliders.Length; i++)
        {
            chunk = colliders[i].transform.parent.GetComponent<Chunk>();
            if (chunk)
            {
                break;
            }
        }
        if (!chunk)
        {
            return;
        }

        m_whenRaisedEvent.Invoke();

        // Add script
        TutorialChunk tChunk = chunk.gameObject.AddComponent<TutorialChunk>();

        tChunk.m_chunkPunchedEvent = m_whenChunkPunched;
        tChunk.m_chunkDestroyedEvent = m_whenChunkDestroyed;

        Destroy(this);
    }

    public void SetActive(bool _active)
    {
        m_enabled = _active;
    }
}
