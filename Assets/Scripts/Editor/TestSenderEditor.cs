using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestSender))]
public class TestSenderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TestSender testSender = (TestSender)target;

        if (GUILayout.Button("Send Message"))
        {
            testSender.TestMessage();
        }

        if (GUILayout.Button("Send Message 2"))
        {
            testSender.TestMessage2();
        }

        base.OnInspectorGUI();
    }
}
