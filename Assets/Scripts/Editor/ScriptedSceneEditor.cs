using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(ScriptedScene)), CanEditMultipleObjects]
public class ScriptedSceneEditor : Editor
{
    ScriptedScene baseRef = null;

    bool[] m_foldouts;

    private void OnEnable()
    {
        baseRef = target as ScriptedScene;

        if (baseRef)
        {
            m_foldouts = new bool[baseRef.m_script.Length];
        }
    }

    public override void OnInspectorGUI()
    {
        if (!baseRef) { return; }
        // Array size
        int arraySize = EditorGUILayout.IntField("Size:", baseRef.m_script.Length);
        if (arraySize != baseRef.m_script.Length)
        {
            // Array size was modified
            UpdateArray<GenericEvent>(ref baseRef.m_script, arraySize);
            UpdateArray<bool>(ref m_foldouts, arraySize);
        }

        // Go through every event
        for (int currentEvent = 0; currentEvent < baseRef.m_script.Length; currentEvent++)
        {
            // Foldout
            m_foldouts[currentEvent] = EditorGUILayout.Foldout(m_foldouts[currentEvent], "Event " + currentEvent.ToString());
            if (m_foldouts[currentEvent])
            {
                // Type
                baseRef.m_script[currentEvent].m_type = (GenericEvent.Type)EditorGUILayout.EnumPopup("Type:", baseRef.m_script[currentEvent].m_type);

                // Update display
                switch (baseRef.m_script[currentEvent].m_type)
                {
                    case GenericEvent.Type.dialogue:
                        {
                            // Attempt cast
                            DialogueEvent dEvent = baseRef.m_script[currentEvent] as DialogueEvent;
                            if (dEvent == null)
                            {
                                // Create a new dialogue
                                baseRef.m_script[currentEvent] = new DialogueEvent();
                                baseRef.m_script[currentEvent].m_type = GenericEvent.Type.dialogue;
                                dEvent = baseRef.m_script[currentEvent] as DialogueEvent;
                            }
                            // Display
                            // Size
                            int size = EditorGUILayout.IntField("Size:", dEvent.m_dialogue.Length);
                            if (size != dEvent.m_dialogue.Length)
                            {
                                UpdateArray<string>(ref dEvent.m_dialogue, size);
                            }
                            // Items
                            for (int i = 0; i < size; i++)
                            {
                                dEvent.m_dialogue[i] = EditorGUILayout.TextField("Text:", dEvent.m_dialogue[i]);
                            }
                            break;
                        }

                    case GenericEvent.Type.animation:
                        {
                            // Attempt cast
                            AnimationEvent aEvent = baseRef.m_script[currentEvent] as AnimationEvent;
                            if (aEvent == null)
                            {
                                // Create a new animation event
                                baseRef.m_script[currentEvent] = new AnimationEvent();
                                baseRef.m_script[currentEvent].m_type = GenericEvent.Type.animation;
                                aEvent = baseRef.m_script[currentEvent] as AnimationEvent;
                            }
                            // Display
                            // Animator
                            aEvent.m_animator = EditorGUILayout.ObjectField(aEvent.m_animator, typeof(Animator), true) as Animator;
                            // Trigger
                            aEvent.m_trigger = EditorGUILayout.TextField("Trigger:", aEvent.m_trigger);
                            break;
                        }

                    default:
                        break;
                }
            }
        }
    }

    private void UpdateArray<T>(ref T[] _array, int _newSize)
    {
        T[] temp = new T[_newSize];

        // Increase
        if (_newSize > _array.Length)
        {
            _array.CopyTo(temp, 0);
        }
        else
        {
            for (int i = 0; i < _newSize; i++)
            {
                temp[i] = _array[i];
            }
        }

        _array = temp;
    }
}
