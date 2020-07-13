using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ScriptedScene : MonoBehaviour
{
    public GenericEvent[] m_script;

    private void Update()
    {
        // Play them in order
    }
}

[System.Serializable]
public class GenericEvent
{
    public enum Type
    {
        none,
        dialogue,
        animation
    }

    public Type m_type = Type.none;
}

public class DialogueEvent : GenericEvent
{
    public string[] m_dialogue = new string[0] { };
}

public class AnimationEvent : GenericEvent
{
    public Animator m_animator = null;
    public string m_trigger = "";
}
