using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScriptedCutscene : MonoBehaviour
{
    [SerializeField] private CutsceneEvent[] m_script;

    private int m_index = -1;
    private bool m_dialogueDone = true;
    private bool m_animationDone = true;

    // Pause
    private float m_timer = 0.0f;
    private bool m_paused = false;

    private void Awake()
    {
        NextEvent();
    }

    private void Update()
    {
        // Run through sequence

        // Current event has Dialogue AND is done being played
        if (HasDialogue(m_script[m_index]) && !m_script[m_index].m_dialogue.IsRunning())
        {
            m_dialogueDone = true; 
        }

        // Current event has Animation AND is done being played
        bool isAnimationRunning = m_script[m_index].m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_script[m_index].m_trigger);
        if (HasAnimator(m_script[m_index]) && !isAnimationRunning)
        {
            m_animationDone = true; 
        }

        // Dialogue and Animation is done
        if (m_dialogueDone && m_animationDone)
        { 
            CheckEventStatus(); 
        }
    }

    private void CheckEventStatus()
    {
        // Set timer
        if (!m_paused && m_script[m_index].m_pause > 0.0f)
        {
            m_paused = true; 
            m_timer = m_script[m_index].m_pause; 
            return; 
        }

        // Timer is still going
        if (m_timer > 0.0f)
        {
            m_timer -= Time.deltaTime; 
            return; 
        }

        NextEvent();
    }

    private void NextEvent()
    {
        m_timer = 0.0f;
        m_paused = false;
        ++m_index;

        // Is at end
        if (m_index >= m_script.Length)
        {
            Destroy(this.gameObject); 
            return; 
        }

        // If blank
        if (!HasDialogue(m_script[m_index]) && !HasAnimator(m_script[m_index]))
        { 
            CheckEventStatus(); 
            return; 
        }

        // Init event(s)
        if (HasDialogue(m_script[m_index]))
        {
            // Dialogue
            m_dialogueDone = false;
            m_script[m_index].m_dialogue.Invoke();
        }
        if (HasAnimator(m_script[m_index]))
        {
            // Animation
            m_animationDone = false;
            m_script[m_index].m_animator.SetTrigger(m_script[m_index].m_trigger);
        }
    }

    private bool HasDialogue(CutsceneEvent _event)
    {
        return _event.m_dialogue;
    }
    private bool HasAnimator(CutsceneEvent _event)
    {
        return _event.m_animator && _event.m_trigger != "";
    }
}

[System.Serializable]
public class CutsceneEvent
{
    [Tooltip("Leave empty if no dialogue is to be played")]
    public Dialogue m_dialogue = null;
    [Tooltip("Leave empty if no animation is to be played")]
    public Animator m_animator = null;
    [Tooltip("Name the trigger and animation the same")]
    public string m_trigger = "";
    [Tooltip("Length of pause after this event (seconds)")]
    public float m_pause = 0.0f;
}
