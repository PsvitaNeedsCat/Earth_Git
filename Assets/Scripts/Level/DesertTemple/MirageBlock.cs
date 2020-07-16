using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageBlock : MonoBehaviour
{
    [SerializeField] private eChunkEffect m_effectType = eChunkEffect.none;

    private MeshRenderer m_renderer = null;
    private eChunkEffect m_currentEffect = eChunkEffect.none;
    private Collider m_collider = null;
    private bool m_attemptToSolidify = false;
    private bool m_isPlayerInside = false;

    private void Awake()
    {
        m_collider = GetComponent<Collider>();
        m_renderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.powerRock, PowerChanged);
        MessageBus.AddListener(EMessageType.powerWater, PowerChanged);
        MessageBus.AddListener(EMessageType.powerFire, PowerChanged);

        // Init update
        Player player = FindObjectOfType<Player>();
        if (player)
        {
            PowerChanged(player.GetCurrentPower().ToString());
        }
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.powerRock, PowerChanged);
        MessageBus.RemoveListener(EMessageType.powerWater, PowerChanged);
        MessageBus.RemoveListener(EMessageType.powerFire, PowerChanged);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if player has entered the block
        if (other.GetComponent<Player>())
        {
            m_isPlayerInside = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Check if player has left the block
        if (other.GetComponent<Player>())
        {
            m_isPlayerInside = false;
        }
    }

    private void Update()
    {
        // If attempting to solidify - wait for player to be outside the block
        if (m_attemptToSolidify && !m_isPlayerInside)
        {
            m_attemptToSolidify = false;
            CanWalkThrough(true);
        }
    }

    // Called when the player changes power
    private void PowerChanged(string _powerName)
    {
        // Convert to enum
        m_currentEffect = StringToEffect(_powerName);

        // Check if chunk is inside
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(0.45f, 0.45f, 0.45f), Vector3.one, Quaternion.identity, 0.0f);
        foreach (RaycastHit hit in hits)
        {
            // If hit a chunk
            if (hit.collider.GetComponentInParent<Chunk>())
            {
                if (!hit.collider.isTrigger)
                {
                    Destroy(hit.collider.transform.parent.gameObject);
                    continue;
                }
                Destroy(hit.collider.gameObject);
            }
        }

        // Try to solidify
        m_attemptToSolidify = m_currentEffect != m_effectType;
        if (!m_attemptToSolidify)
        {
            CanWalkThrough(true);
        }
    }

    // Converts a string to an eChunkEffect
    private eChunkEffect StringToEffect(string _string)
    {
        // Try automatically parsing
        eChunkEffect value = eChunkEffect.none;
        bool result = System.Enum.TryParse(_string, out value);
        if (result)
        {
            return value;
        }

        // Otherwise, convert manually - it is already rock
        if (_string == "powerWater")
        {
            value = eChunkEffect.water;
        }
        else if (_string == "powerFire")
        {
            value = eChunkEffect.fire;
        }

        return value;
    }

    // Updates the mirage block either to be walked through or not
    private void CanWalkThrough(bool _canWalkThrough)
    {
        // Update layer
        gameObject.layer = LayerMask.NameToLayer((m_currentEffect == m_effectType) ? "Ground" : "Walls");

        // Update collider
        m_collider.isTrigger = m_currentEffect == m_effectType;

        // Update alpha
        Color colour = m_renderer.material.color;
        colour.a = (m_currentEffect == m_effectType) ? 0.8f : 1.0f;
        m_renderer.material.color = colour;
    }
}
