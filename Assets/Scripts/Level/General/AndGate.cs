using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class AndGate : MonoBehaviour
{
    [SerializeField] private UnityEvent m_activatedEvent = new UnityEvent();
    [SerializeField] private UnityEvent m_deactivatedEvent = new UnityEvent();

    private bool m_inputA = false;
    private bool m_inputB = false;

    // Updates the first input and activates/deactivates if needed
    public void UpdateInputA(bool _newInput)
    {
        if (m_inputA == _newInput)
        {
            return;
        }

        m_inputA = _newInput;

        UpdateGate();
    }

    // Updates the second input and activates/deactivates if needed
    public void UpdateInputB(bool _newInput)
    {
        if (m_inputB == _newInput)
        {
            return;
        }

        m_inputB = _newInput;

        UpdateGate();
    }

    // Checks if how the gates need to be updated
    private void UpdateGate()
    {
        if (m_inputA && m_inputB)
        {
            m_activatedEvent.Invoke();
        }
        else
        {
            m_deactivatedEvent.Invoke();
        }
    }
}
