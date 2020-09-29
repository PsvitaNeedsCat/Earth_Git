using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatConsole : MonoBehaviour
{
    bool m_showConsole = false;

    string m_input;

    public static CheatCommand AAAA;

    public List<object> m_commandList;

    public void OnToggleDebug()
    {
        m_showConsole = !m_showConsole;
    }

    private void OnGUI()
    {
        if (!m_showConsole)
        {
            return;
        }

        float consoleHeight = 0.0f;

        GUI.Box(new Rect(0, consoleHeight, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        m_input = GUI.TextField(new Rect(10.0f, consoleHeight + 5.0f, Screen.width - 20.0f, 20.0f), m_input);
    }
}
