using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class HealingCrystal : Interactable
{
    // Heals the player
    public GameObject m_crystalMesh;
    public float m_frequency;
    public float m_amplitude;
    public float m_rotationSpeed;

    [SerializeField] private ParticleSystem[] m_activtedEffects = new ParticleSystem[] { };

    private Vector3 m_startPosition;

    public override void Awake()
    {
        base.Awake();
        m_startPosition = m_crystalMesh.transform.position;
    }

    override public void Invoke()
    {
        // Get player
        Player player = FindObjectOfType<Player>();
        Debug.Assert(player, "Could not find player to heal");

        // Get health component
        HealthComponent playerHealth = player.GetComponent<HealthComponent>();
        Debug.Assert(playerHealth, "Player doesn't have a health component");

        // Return to maximum health
        playerHealth.HealToMax();

        // Play sound
        MessageBus.TriggerEvent(EMessageType.crystalHealed);

        // Tween
        m_crystalMesh.transform.DORewind();
        m_crystalMesh.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);

        for(int i = 0; i < m_activtedEffects.Length; i++)
        {
            m_activtedEffects[i].Play();
        }
    }

    public override void Update()
    {
        base.Update();

        m_crystalMesh.transform.position = m_startPosition + Vector3.up * (m_amplitude * Mathf.Sin(Time.time * m_frequency) - (m_amplitude / 2.0f));
        m_crystalMesh.transform.rotation = Quaternion.Euler(0.0f, m_rotationSpeed * Time.deltaTime, 0.0f) * m_crystalMesh.transform.rotation;
    }
}
