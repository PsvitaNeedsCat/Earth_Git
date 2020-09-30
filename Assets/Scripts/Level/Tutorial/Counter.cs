using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class Counter : MonoBehaviour
{
    [SerializeField] private int m_initValue = 1;
    [SerializeField] private bool m_destroyOnComplete = false;
    [SerializeField] private UnityEvent m_completedEvent = new UnityEvent();

    private int m_counter = 0;

    private void Awake()
    {
        m_counter = m_initValue;
    }

    // Counts down by 1
    public void CountDown()
    {
        --m_counter;

        if (m_counter <= 0)
        {
            m_completedEvent.Invoke();

            if (m_destroyOnComplete)
            {
                Destroy(this);
            }
        }
    }
}
