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

        // Animation has two walk cycles in it, start each segment's animation at .25 seconds to offset segments
        for (int i = 0; i < m_segmentAnimators.Count; i++)
        {
            m_segmentAnimators[i].Play("Walk", 0, startTime);
            startTime = (startTime + 0.25f) % 1.0f;
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

    public void Stunned()
    {
        for (int i = 0; i < m_segmentAnimators.Count; i++)
        {
            m_segmentAnimators[i].SetTrigger("Stunned");
        }

        Debug.Log("Set stunned triggers");
    }

    public void Recovered()
    {
        for (int i = 0; i < m_segmentAnimators.Count; i++)
        {
            m_segmentAnimators[i].SetTrigger("Recovered");
        }
    }

    public void ChargeStart()
    {
        m_segmentAnimators[0].SetTrigger("ChargeStart");
    }

    public void ChargeEnd()
    {
        m_segmentAnimators[0].SetTrigger("ChargeEnd");
        // Debug.Log("Charge animation end");
    }

    public void TailAttackStart()
    {
        m_segmentAnimators[6].SetTrigger("TailAttackStart");
    }

    public void TailAttackEnd()
    {
        m_segmentAnimators[6].SetTrigger("TailAttackEnd");
    }
}
