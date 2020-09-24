using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraAnimations : MonoBehaviour
{
    public Animator m_animator;
    public bool m_isReal = false;

    private CobraBoss m_boss;

    private void Awake()
    {
        m_boss = GetComponent<CobraBoss>();
    }

    private void OnEnable()
    {
        if (m_isReal)
        {
            m_animator.SetBool("IsReal", true);
        }
    }

    public void AEFlipTiles()
    {
        if (!m_isReal)
        {
            return;
        }

        m_boss.FlipTiles();
    }

    public void EnterPot()
    {
        m_animator.SetTrigger("EnterPot");
    }

    public void ExitPot()
    {
        m_animator.SetTrigger("ExitPot");
    }

    public void Roar()
    {
        m_animator.SetTrigger("Roar");
    }

    public void MirageWall()
    {
        m_animator.SetTrigger("MirageWall");
    }

    public void CobraJump()
    {
        m_animator.SetTrigger("CobraJump");
    }

    public void LowerHead()
    {
        m_animator.SetTrigger("LowerHead");
    }

    public void RaiseHead()
    {
        m_animator.SetTrigger("RaiseHead");
    }

    public void Damaged()
    {
        m_animator.SetTrigger("Damaged");
    }

    public void PotFire()
    {
        m_animator.SetTrigger("PotFire");
    }

    public void PotJump()
    {
        m_animator.SetTrigger("PotJump");
    }
}
