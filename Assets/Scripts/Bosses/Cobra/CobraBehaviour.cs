using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for cobra behaviours, handles the starting, stopping, and resetting of behaviours
public class CobraBehaviour : MonoBehaviour
{
    public enum EBehaviourState { fresh, running, complete }

    [HideInInspector] public EBehaviourState m_currentState = EBehaviourState.fresh;
    public CobraAnimations m_animations;
    protected static CobraBoss s_boss = null;

    protected virtual void Awake()
    {
        if (s_boss == null)
        {
            s_boss = GetComponent<CobraBoss>();
        }
    }

    private void OnEnable()
    {
        if (s_boss == null)
        {
            s_boss = GetComponent<CobraBoss>();
        }
    }

    private void OnDisable()
    {
        s_boss = null;
    }

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
