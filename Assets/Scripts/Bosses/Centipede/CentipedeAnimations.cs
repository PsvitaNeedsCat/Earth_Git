using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeAnimations : MonoBehaviour
{
    public List<Animator> m_segmentAnimators;
    public AnimationCurve m_speedCurve;

    private void OnEnable()
    {
        float startTime = 0.0f;

        for (int i = 0; i < m_segmentAnimators.Count; i++)
        {
            m_segmentAnimators[i].Play("Walk", 0, startTime);
            startTime = (startTime + 0.25f) % 1.0f;

            //if (i % 2 == 1)
            //{
            //    m_segmentAnimators[i].Play("Walk", 0, 0.75f);
            //}
            //else
            //{
            //    m_segmentAnimators[i].Play("Walk", 0, 0.0f);
            //}
        }
    }

    public void SetAnimSpeed(float _animSpeed)
    {
        float speed = m_speedCurve.Evaluate(_animSpeed);

        foreach (Animator animator in m_segmentAnimators)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}
