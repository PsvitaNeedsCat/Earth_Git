﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBoss : MonoBehaviour
{
    

    public static CentipedeSettings m_settings;
    public List<CentipedeBehaviour> m_behaviourLoop;
    

    private int m_currentBehaviourIndex = 0;
    private int m_totalBehaviours;
    private CentipedeBehaviour m_currentBehaviour;



    private void Awake()
    {
        m_settings = Resources.Load<CentipedeSettings>("ScriptableObjects/CentipedeBossSettings");
        m_totalBehaviours = m_behaviourLoop.Count;

    }

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
        if (m_currentBehaviour.m_currentState == CentipedeBehaviour.EBehaviourState.complete)
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
