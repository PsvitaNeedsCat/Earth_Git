using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadTongueAttack : ToadBehaviour
{
    public Animator m_tongueAnimator;
    public GameObject m_tongueAimIndicator;
    public ToadTongueCollider m_tongueCollider;

    bool m_isRetracting = false;

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        m_toadAnimator.SetTrigger("TongueAttackStart");
        m_tongueAimIndicator.SetActive(true);
    }

    public override void Reset()
    {
        base.Reset();
        m_isRetracting = false;
    }

    private void Update()
    {
        if (!(m_currentState == EBehaviourState.running)) { return; }

        if (m_isRetracting)
        {
            if (m_tongueAnimator.GetCurrentAnimatorStateInfo(0).IsName("ManualTongueExtend_ANIM") && m_tongueAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.01f)
            {
                m_tongueAnimator.SetTrigger("Retract");
            }
        }
    }

    private void Swallow()
    {
        EChunkType typeSwallowed = m_tongueCollider.Swallow();

        ToadBoss.s_eaten = typeSwallowed;

        m_tongueAnimator.gameObject.SetActive(false);
        m_tongueAnimator.SetFloat("ExtendDirection", 1.0f);
        m_tongueAimIndicator.SetActive(false);

        if (typeSwallowed != EChunkType.none)
        {
            MessageBus.TriggerEvent(EMessageType.enemySwallow);
        }
    }

    public void ExtendTongue()
    {
        MessageBus.TriggerEvent(EMessageType.toadTongue);
        m_tongueAnimator.gameObject.SetActive(true);
        m_tongueAnimator.SetTrigger("Extend");
    }

    public void RetractTongue()
    {
        if (m_tongueAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
        {
            // Tongue animation is past halfway, don't reverse animation
            return;
        }

        m_tongueAnimator.SetFloat("ExtendDirection", -1.0f);

        m_isRetracting = true;
    }

    public void OnRetracted()
    {
        m_toadAnimator.SetTrigger("TongueRetracted");
    }

    public void AEExtendTongue()
    {
        ExtendTongue();
    }

    public void AESwallow() => Swallow();
}