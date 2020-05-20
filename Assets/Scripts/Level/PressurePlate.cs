using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private UnityEvent m_activatedEvent;
    [SerializeField] private UnityEvent m_deactivatedEvent;

    // Holds all objects currently on the preasure plate
    private List<GameObject> m_objects = new List<GameObject>();

    private void OnEnable() => MessageBus.AddListener(EMessageType.chunkDestroyed, ChunkWasDestroyed);
    private void OnDisable() => MessageBus.RemoveListener(EMessageType.chunkDestroyed, ChunkWasDestroyed);

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player) { AddObject(other.gameObject); }

        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk) { AddObject(other.gameObject); }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player) { RemoveObject(other.gameObject); }

        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk) { RemoveObject(other.gameObject); }
    }

    // Called when a chunk/player goes on the preasure plate
    private void AddObject(GameObject _go)
    {
        if (m_objects.Count == 0) { m_activatedEvent.Invoke(); }

        m_objects.Add(_go);
    }

    // Called when a chunk/player leaves the preasure plate
    private void RemoveObject(GameObject _go)
    {
        if (m_objects.Contains(_go))
        {
            m_objects.Remove(_go);
        }

        if (m_objects.Count == 0) { m_deactivatedEvent.Invoke(); }
    }

    // Called when a chunk is destroyed - checks if it was on the preasure plate
    private void ChunkWasDestroyed(string _null)
    {
        // Check that all the chunks are still valid
        foreach (GameObject go in m_objects)
        {
            if (go.GetComponentInParent<Chunk>())
            {
                RemoveObject(go);
                return;
            }
        }
    }
}
