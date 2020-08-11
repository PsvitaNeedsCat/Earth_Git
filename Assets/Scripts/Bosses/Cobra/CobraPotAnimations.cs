using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraPotAnimations : MonoBehaviour
{
    public Animator m_animator;

    public void PotFire()
    {
        m_animator.SetTrigger("PotFire");
    }

    public void PotJump()
    {
        m_animator.SetTrigger("PotJump");
    }

    public void EnterPot()
    {
        m_animator.SetTrigger("EnterPot");
    }

    public void ExitPot()
    {
        m_animator.SetTrigger("ExitPot");
    }

    public void LowerHead()
    {
        m_animator.SetTrigger("LowerHead");
    }

    public void RaiseHead()
    {
        m_animator.SetTrigger("RaiseHead");
    }
}
