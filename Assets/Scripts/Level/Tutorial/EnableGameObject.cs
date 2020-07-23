using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGameObject : MonoBehaviour
{
    [SerializeField] private GameObject m_object = null;

    public void EnbaleObject()
    {
        m_object.SetActive(true);
    }
}
