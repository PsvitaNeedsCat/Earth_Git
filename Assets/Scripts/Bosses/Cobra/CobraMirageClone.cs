using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageClone : MonoBehaviour
{
    private BoxCollider m_collider;
    private CobraMirageSpit m_spit;

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider>();
        m_spit = GetComponent<CobraMirageSpit>();
    }

    public void Damage()
    {
        MessageBus.TriggerEvent(EMessageType.cobraMirageDamaged);
        m_spit.RaiseHead();
    }
}
