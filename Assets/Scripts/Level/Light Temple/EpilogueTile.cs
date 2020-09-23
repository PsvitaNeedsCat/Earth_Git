using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class EpilogueTile : MonoBehaviour
{
    [SerializeField] private UnityEvent m_chunkRaisedEvent = new UnityEvent();
    [SerializeField] private UnityEvent m_chunkPunchedEvent = new UnityEvent();

    // Add and remove triggers for events
    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.chunkRaise, ChunkRaised);
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.chunkRaise, ChunkRaised);
    }

    // Sets the chunk as an epilogue chunk
    private void ChunkRaised(string _null)
    {
        m_chunkRaisedEvent.Invoke();

        GameObject chunkObj = FindObjectOfType<Chunk>().gameObject;

        Destroy(chunkObj.GetComponent<Chunk>());
        Destroy(chunkObj.GetComponent<Rigidbody>());
        Destroy(chunkObj.GetComponent<HealthComponent>());

        GameObject mainColliderObj = chunkObj.transform.GetChild(0).gameObject;

        Rigidbody chunkRigidBody = mainColliderObj.AddComponent<Rigidbody>();
        chunkRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        chunkRigidBody.angularDrag = 0.0f;

        EpilogueChunk mainColliderEpilogue = mainColliderObj.AddComponent<EpilogueChunk>();
        mainColliderEpilogue.m_completedMoveEvent = m_chunkPunchedEvent;

        Destroy(GetComponent<Tile>());
        Destroy(this);
    }
}
