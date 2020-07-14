using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScriptedCutscene : MonoBehaviour
{
    [SerializeField] private CutsceneEvent[] m_script;

    private int m_index = -1;

    private void Awake()
    {
        NextEvent();
    }

    private void Update()
    {
        // Run through sequence
        
        // Dialogue

    }

    private void NextEvent()
    {
        ++m_index;

        // If blank
        if (!m_script[m_index].m_dialogue &&
            (!m_script[m_index].m_animator || m_script[m_index].m_trigger == ""))
        { NextEvent(); return; }

        // Init event(s)
        if (m_script[m_index].m_dialogue)
        {
            // Dialogue
            m_script[m_index].m_dialogue.Invoke();
        }
        if (m_script[m_index].m_animator && m_script[m_index].m_trigger != "")
        {
            // Animation
            m_script[m_index].m_animator.SetTrigger(m_script[m_index].m_trigger);
        }
    }
}

[System.Serializable]
public class CutsceneEvent
{
    public Dialogue m_dialogue = null;
    public Animator m_animator = null;
    public string m_trigger = "";
}
