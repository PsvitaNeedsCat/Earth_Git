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

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.chunkDestroyed, ChunkWasDestroyed);
        MessageBus.AddListener(EMessageType.glassDestroyed, GlassWasDestroyed);
        m_activatedEvent.AddListener(() => MessageBus.TriggerEvent(EMessageType.pressurePlateOn));
        m_deactivatedEvent.AddListener(() => MessageBus.TriggerEvent(EMessageType.pressurePlateOff));
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.chunkDestroyed, ChunkWasDestroyed);
        MessageBus.RemoveListener(EMessageType.glassDestroyed, GlassWasDestroyed);
        m_activatedEvent.RemoveListener(() => MessageBus.TriggerEvent(EMessageType.pressurePlateOn));
        m_deactivatedEvent.RemoveListener(() => MessageBus.TriggerEvent(EMessageType.pressurePlateOff));
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            AddObject(other.gameObject);
            return;
        }

        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk)
        {
            AddObject(other.gameObject);
            return;
        }

        SandBlock sand = other.GetComponent<SandBlock>();
        if (sand)
        {
            AddObject(other.gameObject);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            RemoveObject(other.gameObject);
            return;
        }

        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk)
        {
            RemoveObject(other.gameObject);
            return;
        }

        SandBlock sand = other.GetComponent<SandBlock>();
        if (sand)
        {
            RemoveObject(other.gameObject);
            return;
        }
    }

    // Called when a chunk/player goes on the preasure plate
    private void AddObject(GameObject _go)
    {
        if (m_objects.Count == 0)
        {
            m_activatedEvent.Invoke();
        }

        m_objects.Add(_go);
    }

    // Called when a chunk/player leaves the preasure plate
    private void RemoveObject(GameObject _go)
    {
        if (m_objects.Contains(_go))
        {
            m_objects.Remove(_go);
        }

        if (m_objects.Count == 0)
        {
            m_deactivatedEvent.Invoke();
        }
    }

    // Called when a chunk is destroyed - checks if it was on the preasure plate
    private void ChunkWasDestroyed(string _null)
    {
        foreach (GameObject go in m_objects)
        {
            Chunk chunk = go.GetComponentInParent<Chunk>();
            if (chunk && chunk.m_isBeingDestoyed)
            {
                RemoveObject(go);
                return;
            }
        }
    }

    // Same as ChunkWasDestroyed but for glass
    private void GlassWasDestroyed(string _null)
    {
        // Check that all the sand is still valid
        foreach (GameObject go in m_objects)
        {
            if (go.GetComponent<SandBlock>())
            {
                RemoveObject(go);
                return;
            }
        }
    }
}
