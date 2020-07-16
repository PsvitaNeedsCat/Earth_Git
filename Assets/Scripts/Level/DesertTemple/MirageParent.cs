using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageParent : MonoBehaviour
{
    [SerializeField] private string m_defaultLayer = "Walls";

    public eChunkEffect m_effectType = eChunkEffect.none;

    protected eChunkEffect m_currentEffect = eChunkEffect.none;
    private MeshRenderer m_renderer = null;
    private Collider m_collider = null;

    protected virtual void Awake()
    {
        m_collider = GetComponent<Collider>();
        m_renderer = GetComponent<MeshRenderer>();
    }

    protected virtual void OnEnable()
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
    protected virtual void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.powerRock, PowerChanged);
        MessageBus.RemoveListener(EMessageType.powerWater, PowerChanged);
        MessageBus.RemoveListener(EMessageType.powerFire, PowerChanged);
    }

    // Called when the player changes power
    public virtual void PowerChanged(string _powerName)
    {
        // Convert to enum
        m_currentEffect = StringToEffect(_powerName);

        // Solidify / Unsolidify
        CanWalkThrough(m_currentEffect == m_effectType);
    }

    // Converts a string to an eChunkEffect
    protected eChunkEffect StringToEffect(string _string)
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
    protected virtual void CanWalkThrough(bool _canWalkThrough)
    {
        // Update layer
        gameObject.layer = LayerMask.NameToLayer((m_currentEffect == m_effectType) ? "Ground" : m_defaultLayer);

        // Update collider
        m_collider.isTrigger = m_currentEffect == m_effectType;

        // Update alpha
        Color colour = m_renderer.material.color;
        colour.a = (m_currentEffect == m_effectType) ? 0.8f : 1.0f;
        m_renderer.material.color = colour;
    }
}
