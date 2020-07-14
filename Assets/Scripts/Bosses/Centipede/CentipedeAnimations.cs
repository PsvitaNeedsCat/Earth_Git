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

    // Sets the animation speed of all centipede segments
    public void SetAnimSpeed(float _animSpeed)
    {
        float speed = m_speedCurve.Evaluate(_animSpeed);

        foreach (Animator animator in m_segmentAnimators)
        {
            animator.SetFloat("Speed", speed);
        }
    }

    // Sets the stunned trigger on all body segment animators
    public void Stunned()
    {
        for (int i = 0; i < m_segmentAnimators.Count; i++)
        {
            m_segmentAnimators[i].SetTrigger("Stunned");
        }
    }

    // Sets the recovered trigger on all body segment animators
    public void Recovered()
    {
        for (int i = 0; i < m_segmentAnimators.Count; i++)
        {
            m_segmentAnimators[i].SetTrigger("Recovered");
        }
    }

    // Start the charging animation on the head segment
    public void ChargeStart()
    {
        m_segmentAnimators[0].SetTrigger("ChargeStart");
    }

    // End the charging animatino on the head segment
    public void ChargeEnd()
    {
        m_segmentAnimators[0].SetTrigger("ChargeEnd");
    }

    // Start the tail attack animation on the tail segment
    public void TailAttackStart()
    {
        m_segmentAnimators[6].SetTrigger("TailAttackStart");
    }

    // End the tail attack animation on the tail segment
    public void TailAttackEnd()
    {
        m_segmentAnimators[6].SetTrigger("TailAttackEnd");
    }
}
