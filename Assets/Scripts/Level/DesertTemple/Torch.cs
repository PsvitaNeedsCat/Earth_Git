using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class Torch : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_fireParticles = null;
    [SerializeField] private bool m_active = false;
    [SerializeField] private ParticleSystem m_activatedParticles = null;
    [SerializeField] private ParticleSystem m_deactivatedParticles = null;

    [SerializeField] private Material m_activeMaterial;
    [SerializeField] private Material m_inactiveMaterial;
    private MeshRenderer m_meshRenderer = null;

    [SerializeField] private UnityEvent m_activatedEvent = new UnityEvent();
    [SerializeField] private UnityEvent m_deactivatedEvent = new UnityEvent();

    private void Awake()
    {
        m_fireParticles = GetComponentInChildren<ParticleSystem>();

        m_meshRenderer = GetComponentInChildren<MeshRenderer>();


        UpdateMaterial();
    }

    private void OnEnable()
    {
        if (m_active)
        {
            m_fireParticles.Play();
        }
        else
        {
            m_fireParticles.Stop();
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
            m_activatedParticles.Play();

            UpdateMaterial();
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
            m_deactivatedParticles.Play();

            UpdateMaterial();
        }
    }

    private void UpdateMaterial()
    {
        m_meshRenderer.material = (m_active) ? m_activeMaterial : m_inactiveMaterial;
    }
}
