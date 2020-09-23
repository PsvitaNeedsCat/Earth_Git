using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollisionEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent m_triggerEvent = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            m_triggerEvent.Invoke();
            Destroy(gameObject);
        }
    }
}
