using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using DG.Tweening;

public class WallButton : MonoBehaviour
{
    [SerializeField] private UnityEvent m_triggerEvent;

    public void Invoke()
    {
        m_triggerEvent.Invoke();

        Vector3 pos = transform.position;
        Vector3 halfForward = transform.forward * 0.5f;
        transform.DORewind();
        transform.DOMove(pos - halfForward, 0.5f).OnComplete(() => transform.DOMove(pos + halfForward, 0.5f));
    }
}
