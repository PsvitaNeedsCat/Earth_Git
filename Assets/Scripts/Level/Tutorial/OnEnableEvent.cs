using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class OnEnableEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent m_enableEvent = new UnityEvent();

    private void OnEnable()
    {
        m_enableEvent.Invoke();
    }
}
