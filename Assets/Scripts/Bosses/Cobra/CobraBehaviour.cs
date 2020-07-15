using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraBehaviour : MonoBehaviour
{
    public enum EBehaviourState { fresh, running, complete }

    [HideInInspector] public EBehaviourState m_currentState = EBehaviourState.fresh;
    public CobraAnimations m_animations;

    public virtual void StartBehaviour()
    {
        m_currentState = EBehaviourState.running;
    }

    public virtual void Reset()
    {
        m_currentState = EBehaviourState.fresh;
    }

    public virtual void CompleteBehaviour()
    {
        m_currentState = EBehaviourState.complete;
    }

    protected IEnumerator CompleteAfterSeconds(float _afterSeconds)
    {
        yield return new WaitForSeconds(_afterSeconds);
        Debug.Log("Completed behaviour " + this.GetType().Name);
        CompleteBehaviour();
    }
}
