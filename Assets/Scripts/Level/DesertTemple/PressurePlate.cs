using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private UnityEvent m_activatedEvent;
    [SerializeField] private UnityEvent m_deactivatedEvent;
    private float m_moveAmount = 0.05f;

    private MeshRenderer m_renderer;
    private float m_rendererY;
    private Vector3 m_inactivePosition;
    private Vector3 m_activePosition;
    private bool m_active = false;

    // Holds all objects currently on the pressure plate
    private List<GameObject> m_objects = new List<GameObject>();

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.chunkDestroyed, ChunkWasDestroyed);
        MessageBus.AddListener(EMessageType.waterChunkDestroyed, ChunkWasDestroyed);
        MessageBus.AddListener(EMessageType.fieryExplosion, ChunkWasDestroyed);
        MessageBus.AddListener(EMessageType.glassDestroyed, GlassWasDestroyed);
        MessageBus.AddListener(EMessageType.chunkHit, CheckChunkForMovement);
        m_activatedEvent.AddListener(() => MessageBus.TriggerEvent(EMessageType.pressurePlateOn));
        m_deactivatedEvent.AddListener(() => MessageBus.TriggerEvent(EMessageType.pressurePlateOff));
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.chunkDestroyed, ChunkWasDestroyed);
        MessageBus.RemoveListener(EMessageType.waterChunkDestroyed, ChunkWasDestroyed);
        MessageBus.RemoveListener(EMessageType.fieryExplosion, ChunkWasDestroyed);
        MessageBus.RemoveListener(EMessageType.glassDestroyed, GlassWasDestroyed);
        MessageBus.RemoveListener(EMessageType.chunkHit, CheckChunkForMovement);
        m_activatedEvent.RemoveListener(() => MessageBus.TriggerEvent(EMessageType.pressurePlateOn));
        m_deactivatedEvent.RemoveListener(() => MessageBus.TriggerEvent(EMessageType.pressurePlateOff));
    }

    private void Awake()
    {
        m_renderer = GetComponentInChildren<MeshRenderer>();
        m_inactivePosition = m_renderer.transform.position;
        m_activePosition = m_renderer.transform.position + Vector3.down * m_moveAmount;
    }

    private void Update()
    {
        if (!m_active)
        {
            float blendVal = (Mathf.Sin(Time.time) + 1.0f) / 10.0f; // 25.0f
            m_renderer.material.SetFloat("_TextureBlend", blendVal);
        }
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
            OnActivated();
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
            OnDeactivated();
        }
    }

    private void OnActivated()
    {
        m_active = true;

        DOTween.Kill(this);
        DOTween.To(() => m_renderer.material.GetFloat("_TextureBlend"), x => m_renderer.material.SetFloat("_TextureBlend", x), 1.0f, 0.5f).SetEase(Ease.OutSine);

        m_renderer.transform.DOMove(m_activePosition, 0.5f).SetEase(Ease.OutBounce);
    }

    private void OnDeactivated()
    {
        m_active = false;

        DOTween.Kill(this);
        DOTween.To(() => m_renderer.material.GetFloat("_TextureBlend"), x => m_renderer.material.SetFloat("_TextureBlend", x), 0.0f, 0.5f).SetEase(Ease.OutSine);

        m_renderer.transform.DOMove(m_inactivePosition, 0.5f).SetEase(Ease.OutBounce);
    }

    // Called when a chunk is destroyed - checks if it was on the preasure plate
    private void ChunkWasDestroyed(string _null)
    {
        foreach (GameObject go in m_objects)
        {
            if (!go)
            {
                RemoveObject(go);
                return;
            }

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
            if (!go || go.GetComponent<SandBlock>())
            {
                RemoveObject(go);
                return;
            }
        }
    }

    private void CheckChunkForMovement(string _null)
    {
        Vector3 centre = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        Collider[] colliders = Physics.OverlapBox(centre, new Vector3(0.45f, 0.45f, 0.45f));
        foreach (Collider collider in colliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();
            if (chunk)
            {
                if (!chunk.GetComponent<Rigidbody>().isKinematic)
                {
                    RemoveObject(chunk.transform.GetChild(0).gameObject);
                }
            }
        }
    }
}
