using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class Torch : MonoBehaviour
{
    [SerializeField] private UnityEvent m_activatedEvent = new UnityEvent();
    [SerializeField] private UnityEvent m_deactivatedEvent = new UnityEvent();
    [SerializeField] private bool m_active = false;
    private ParticleSystem m_fireParticles = null;

    private void Awake()
    {
        m_fireParticles = GetComponentInChildren<ParticleSystem>();

        if (m_active)
        {
            m_fireParticles.Play();
        }
    }

    // Called by chunk - checks if already activated, and activates it if required
    public void AttemptToActivate()
    {
        if (!m_active)
        {
            m_active = true;
            m_activatedEvent.Invoke();
            m_fireParticles.Play();
        }
    }

    // Called by chunk - checks if already activated, and deactivates it if required
    public void AttemptToDeactivate()
    {
        if (m_active)
        {
            m_active = false;
            m_deactivatedEvent.Invoke();
            m_fireParticles.Stop();
        }
    }
}
