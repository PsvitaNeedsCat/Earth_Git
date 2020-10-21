using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Cinemachine;
public class TweenToHere : MonoBehaviour
{
    [SerializeField] private GameObject m_tweenedObject = null;
    [SerializeField] private float m_tweenTime = 0.0f;
    [SerializeField] private Ease m_moveEaseType = Ease.Linear;
    [SerializeField] private bool m_onStart = false;

    private void Start()
    {
        if (m_onStart)
        {
            TweenNoRotation();
        }
    }

    // Tweens the object here over a time and then destroys the script
    public void Tween()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(m_tweenedObject.transform.DOMove(transform.position, m_tweenTime).SetEase(m_moveEaseType));
        seq.Insert(0.0f, m_tweenedObject.transform.DORotateQuaternion(transform.rotation, m_tweenTime));
        seq.Play();
    }

    public void TweenNoRotation()
    {
        m_tweenedObject.transform.DOMove(transform.position, m_tweenTime).SetEase(m_moveEaseType);
    }
}
