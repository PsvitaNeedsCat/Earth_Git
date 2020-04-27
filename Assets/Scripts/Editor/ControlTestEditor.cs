using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControlTest))]
public class ControlTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ControlTest test = (ControlTest)target;

        if (GUILayout.Button("Toggle Movement"))
        {
            test.MovementToggle();
        }

        if (GUILayout.Button("Toggle Combat"))
        {
            test.CombatToggle();
        }

        base.OnInspectorGUI();
    }
}
