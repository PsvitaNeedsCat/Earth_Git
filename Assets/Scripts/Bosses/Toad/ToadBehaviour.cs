using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadBehaviour : MonoBehaviour
{
    public enum EBehaviourState
    {
        fresh, // Been reset, ready to go
        running, // Currently running
        complete // Has finished running, but not been reset
    }

    //[HideInInspector]
    public EBehaviourState m_currentState = EBehaviourState.fresh;
    public Animator m_toadAnimator;

    public virtual void StartBehaviour()
    {
        m_currentState = EBehaviourState.running;
    }

    public virtual void Reset()
    {
        m_currentState = EBehaviourState.fresh;
    }

    protected IEnumerator CompleteAfterSeconds(float _afterSeconds)
    {
        yield return new WaitForSeconds(_afterSeconds);

        m_currentState = EBehaviourState.complete;
    }

    public void AEBehaviourComplete()
    {
        m_currentState = EBehaviourState.complete;
    }
}
