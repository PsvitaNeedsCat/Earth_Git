using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToadWave : MonoBehaviour
{
    public float m_travelTime = 3.0f;
    public float m_travelDist = 12.0f;

    public void Launch()
    {
        MessageBus.TriggerEvent(EMessageType.swampWave);

        ActivateChildren();

        transform.DOMoveZ(transform.position.z + m_travelDist, m_travelTime).OnComplete(() => WaveEnd());
    }

    void WaveEnd()
    {
        this.gameObject.SetActive(false);
    }

    void ActivateChildren()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    void DeactivateChildren()
    {
        foreach(Transform child  in this.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
