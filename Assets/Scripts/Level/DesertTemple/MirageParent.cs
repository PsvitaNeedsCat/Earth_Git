using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageParent : MonoBehaviour
{
    [SerializeField] private string m_defaultLayer = "Walls";

    public EChunkEffect m_effect = EChunkEffect.none;

    protected EChunkEffect m_playerEffect = EChunkEffect.none;
    protected MeshRenderer m_renderer = null;
    protected Collider m_collider = null;

    protected virtual void Awake()
    {
        m_collider = GetComponent<Collider>();
        m_renderer = GetComponent<MeshRenderer>();
        m_renderer.material = new Material(m_renderer.material);
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
        m_playerEffect = StringToEffect(_powerName);

        // Solidify / Unsolidify
        CanWalkThrough(m_playerEffect == m_effect);
    }

    // Converts a string to an eChunkEffect
    protected EChunkEffect StringToEffect(string _string)
    {
        // Try automatically parsing
        EChunkEffect value = EChunkEffect.none;
        bool result = System.Enum.TryParse(_string, out value);
        if (result)
        {
            return value;
        }

        // Otherwise, convert manually - it is already rock
        if (_string == "powerWater")
        {
            value = EChunkEffect.water;
        }
        else if (_string == "powerFire")
        {
            value = EChunkEffect.fire;
        }

        return value;
    }

    // Updates the mirage block either to be walked through or not
    protected virtual void CanWalkThrough(bool _canWalkThrough)
    {
        // Update layer
        gameObject.layer = LayerMask.NameToLayer((m_playerEffect == m_effect) ? "Ground" : m_defaultLayer);

        // Update collider
        m_collider.isTrigger = (m_playerEffect == m_effect);

        // float currentValue = (_canWalkThrough) ? 0.0f : 1.0f;
        float endValue = (_canWalkThrough) ? 1.0f : 0.0f;
        // float changeRate = (_canWalkThrough) ? 2.0f : -2.0f;

        // StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_renderer.material, "_Cutoff", currentValue, endValue, changeRate, _canWalkThrough));
        StopAllCoroutines();
        StartCoroutine(BossHelper.ChangeMaterialFloatPropertyOver(m_renderer.material, "_Cutoff", endValue, 0.5f));
    }
}
