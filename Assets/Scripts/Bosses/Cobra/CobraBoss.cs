using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraBoss : MonoBehaviour
{
    public static CobraGlobalSettings m_settings;

    public List<CobraBehaviour> m_behaviourLoop;

    private int m_currentBehaviourIndex = 0;
    private int m_totalBehaviours;
    private CobraBehaviour m_currentBehaviour;
    private CobraHealth m_cobraHealth;

    // Initialise variables
    private void Awake()
    {
        m_cobraHealth = GetComponent<CobraHealth>();

        m_settings = Resources.Load<CobraGlobalSettings>("ScriptableObjects/CobraGlobalSettings");
        m_cobraHealth.SetCurrentHealth(m_settings.m_maxHealth);

        m_totalBehaviours = m_behaviourLoop.Count;
    }

    // Start first behaviour
    private void Start()
    {
        m_currentBehaviour = m_behaviourLoop[0];
        m_currentBehaviour.StartBehaviour();
    }

    private void Update()
    {
        UpdateBehaviour();
    }

    private void UpdateBehaviour()
    {
        // Move to next behaviour when complete
        if (m_currentBehaviour.m_currentState == CobraBehaviour.EBehaviourState.complete)
        {
            GoToNextBehaviour();
        }
    }

    private void GoToNextBehaviour()
    {
        // Reset behaviour we just finished
        m_currentBehaviour.Reset();

        // Move to next behaviour and start it
        m_currentBehaviourIndex = (m_currentBehaviourIndex + 1) % m_totalBehaviours;
        m_currentBehaviour = m_behaviourLoop[m_currentBehaviourIndex];
        m_currentBehaviour.StartBehaviour();
    }
}
