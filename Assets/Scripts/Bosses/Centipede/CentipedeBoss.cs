using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles behaviour sequencing of centipede boss
public class CentipedeBoss : MonoBehaviour
{
    public static CentipedeSettings s_settings;
    public static bool s_dropLava = false;
    public List<CentipedeBehaviour> m_behaviourLoop;

    private int m_currentBehaviourIndex = 0;
    private int m_totalBehaviours;
    private int m_behaviourLoopCount = 0;
    private CentipedeBehaviour m_currentBehaviour;
    private CentipedeHealth m_centipedeHealth;

    // Initialise variables
    private void Awake()
    {
        s_settings = Resources.Load<CentipedeSettings>("ScriptableObjects/CentipedeBossSettings");
        m_totalBehaviours = m_behaviourLoop.Count;
        s_dropLava = false;
        m_centipedeHealth = GetComponent<CentipedeHealth>();
    }

    // Start first behaviour
    private void Start()
    {
        m_currentBehaviour = m_behaviourLoop[0];
        m_currentBehaviour.StartBehaviour();
        MessageBus.TriggerEvent(EMessageType.centipedeSpawn);
    }

    private void Update()
    {
        UpdateBehaviour();
    }

    private void UpdateBehaviour()
    {
        // Move to next state when complete
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

        // Behaviours have looped
        if (m_currentBehaviourIndex == 0)
        {
            m_behaviourLoopCount++;
        }

        List<int> damagedSegments = m_centipedeHealth.GetDamagedSegments();

        // If two segments are damaged, do undamaged attack and then only one other
        if (m_centipedeHealth.GetHealth() == 1)
        {
            // If behaviour loop count is even, skip first damaged attack
            if (m_currentBehaviourIndex == damagedSegments[0] && (m_behaviourLoopCount % 2) == 0)
            {
                SkipBehaviour();
            }
            // If behaviour loop count is odd, skip second damaged attack
            else if (m_currentBehaviourIndex == damagedSegments[1] && (m_behaviourLoopCount % 2) == 1)
            {
                SkipBehaviour();
            }
        }
        // If one segment is damaged, alternate between undamaged attacks
        else if (m_centipedeHealth.GetHealth() == 2)
        {
            // If we are about to do the damaged attack, skip it
            if (m_currentBehaviourIndex == (damagedSegments[0]))
            {
                SkipBehaviour();
            }
        }

        m_currentBehaviour = m_behaviourLoop[m_currentBehaviourIndex];
        m_currentBehaviour.StartBehaviour();
    }

    private void SkipBehaviour()
    {
        m_currentBehaviourIndex = (m_currentBehaviourIndex + 1) % m_totalBehaviours;

        // Behaviours have looped
        if (m_currentBehaviourIndex == 0)
        {
            m_behaviourLoopCount++;
        }
    }
}
