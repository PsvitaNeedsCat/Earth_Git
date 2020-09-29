using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
    [Tooltip("Calls this function when the player collides with the trigger")]
    [SerializeField] private UnityEvent m_triggeredEvent = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            m_triggeredEvent.Invoke();
        }
    }
}
