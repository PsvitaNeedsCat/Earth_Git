﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Cinemachine;
public class TweenToHere : MonoBehaviour
{
    [SerializeField] private GameObject m_tweenedObject = null;
    [SerializeField] private float m_tweenTime = 0.0f;

    // Tweens the object here over a time and then destroys the script
    public void Tween()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(m_tweenedObject.transform.DOMove(transform.position, m_tweenTime));
        seq.Insert(0.0f, m_tweenedObject.transform.DORotateQuaternion(transform.rotation, m_tweenTime));
        seq.OnComplete(() => Destroy(this));
        seq.Play();

        CinemachineImpulseSource src = new CinemachineImpulseSource();

        
    }
}
