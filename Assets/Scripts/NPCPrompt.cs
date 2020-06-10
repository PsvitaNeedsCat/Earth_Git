using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NPCPrompt : MonoBehaviour
{
    public GameObject m_buttonPrompt;

    private void Awake()
    {
        m_buttonPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            m_buttonPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            m_buttonPrompt.SetActive(false);
        }
    }
}
