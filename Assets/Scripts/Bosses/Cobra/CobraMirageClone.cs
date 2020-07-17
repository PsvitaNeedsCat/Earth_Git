using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageClone : MonoBehaviour
{
    private BoxCollider m_collider;

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider>();
    }

    public void LowerHead()
    {
        m_collider.enabled = true;
    }

    public void RaiseHead()
    {
        m_collider.enabled = false;
    }
}
