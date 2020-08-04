using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageClone : MonoBehaviour
{
    private BoxCollider m_collider;

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider>();
    }

    public void Damage()
    {
        MessageBus.TriggerEvent(EMessageType.cobraMirageDamaged);
        Destroy(this.gameObject);
    }
}
