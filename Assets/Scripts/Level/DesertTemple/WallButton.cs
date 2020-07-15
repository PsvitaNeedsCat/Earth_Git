using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using DG.Tweening;

public class WallButton : MonoBehaviour
{
    [SerializeField] private UnityEvent m_triggerEvent;

    private Vector3 m_startPos;

    private void Awake()
    {
        m_startPos = transform.position;
    }

    public void Invoke()
    {
        m_triggerEvent.Invoke();
        
        Vector3 halfForward = transform.forward * 0.05f;
        transform.DOKill();
        transform.position = m_startPos;
        transform.DOMove(transform.position - halfForward, 0.5f).OnComplete(() => transform.DOMove(transform.position + halfForward, 0.5f));
    }
}
