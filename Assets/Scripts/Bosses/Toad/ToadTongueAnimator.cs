using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadTongueAnimator : MonoBehaviour
{
    public ToadTongueCollider m_tongueCollider;

    public void OnRetracted()
    {
        m_tongueCollider.OnRetracted();
        Debug.Log("OnRetracted");
    }
}
