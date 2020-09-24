using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Rotation : MonoBehaviour
{
    private enum Axis
    {
        x,
        y,
        z,
    }
    [SerializeField] private Axis m_rotationAxis = Axis.y;

    private float m_rotationSpeed = 30.0f;
    private float m_xModifier;
    private float m_yModifier;
    private float m_zModifier;

    private void Awake()
    {
        m_xModifier = (m_rotationAxis == Axis.x) ? m_rotationSpeed : 0.0f;
        m_yModifier = (m_rotationAxis == Axis.y) ? m_rotationSpeed : 0.0f;
        m_zModifier = (m_rotationAxis == Axis.z) ? m_rotationSpeed : 0.0f;
    }

    private void OnEnable()
    {
        //transform.DORotate(m_rotationVector, 3.0f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);

        StartCoroutine(Spin());
    }

    private IEnumerator Spin()
    {
        while (true)
        {
            transform.rotation = Quaternion.Euler(Time.time * m_xModifier, Time.time * m_yModifier, Time.time * m_zModifier);

            yield return null;
        }
    }
}
