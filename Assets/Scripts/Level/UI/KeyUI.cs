using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUI : MonoBehaviour
{
    [SerializeField] private GameObject[] m_keyIcons = new GameObject[3];
    private Player m_playerRef = null;

    // Updates how many sprites are active based on how many keys the player has
    public void UpdateIcons()
    {
        if (!m_playerRef)
        {
            m_playerRef = FindObjectOfType<Player>();
        }

        int keyCount = m_playerRef.m_collectedKeys.Count;

        for (int i = 0; i < 3; i++)
        {
            m_keyIcons[i].SetActive(keyCount > i);
        }
    }
}
