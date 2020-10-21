using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTile : MonoBehaviour
{
    [SerializeField] private int[] m_ids = { };
    [SerializeField] private GameObject m_symbol = null;
    private DoorManager m_doorManager = null;

    private void Awake()
    {
        m_doorManager = FindObjectOfType<DoorManager>();
    }

    private void OnEnable()
    {
        CheckKeyStatus();
    }

    private void Start()
    {
        CheckKeyStatus();
    }

    private void CheckKeyStatus()
    {
        if (m_doorManager)
        {
            for (int i = 0; i < m_ids.Length; i++)
            {
                if (!m_doorManager.HasKeyBeenCollected(m_ids[i]))
                {
                    return;
                }
            }

            m_symbol.SetActive(true);
        }
    }
}
