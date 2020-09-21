using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUI : MonoBehaviour
{
    [SerializeField] private GameObject[] m_baseKeySprites = new GameObject[3];
    [SerializeField] private GameObject[] m_waterKeySprites = new GameObject[3];
    [SerializeField] private GameObject[] m_fireKeySprites = new GameObject[3];
    [SerializeField] private GameObject[] m_sandKeySprites = new GameObject[3];
    
    private List<Key.Type> m_keyTypes = new List<Key.Type>();

    // Called when the player collects a key - adds it to the list and updates the UI
    public void KeyCollected(Key.Type _keyType)
    {
        m_keyTypes.Add(_keyType);

        UpdateIcons();
    }

    // Called when a key has been removed from the player - finds the key type in list and removes it. Updates UI
    public void KeyRemoved(Key.Type _keyType)
    {
        if (m_keyTypes.Contains(_keyType))
        {
            for (int i = 0; i < m_keyTypes.Count; i++)
            {
                if (m_keyTypes[i] == _keyType)
                {
                    m_keyTypes.RemoveAt(i);
                    UpdateIcons();
                    break;
                }
            }
        }
    }

    // Updates how many sprites are active based on how many keys the player has
    private void UpdateIcons()
    {
        DisableAllUI();

        for (int i = 0; i < m_keyTypes.Count; i++)
        {
            switch (m_keyTypes[i])
            {
                case Key.Type.fireBoss:
                    {
                        m_fireKeySprites[i].SetActive(true);
                        break;
                    }

                case Key.Type.waterBoss:
                    {
                        m_waterKeySprites[i].SetActive(true);
                        break;
                    }

                case Key.Type.sandBoss:
                    {
                        m_sandKeySprites[i].SetActive(true);
                        break;
                    }

                default: // Basic
                    {
                        m_baseKeySprites[i].SetActive(true);
                        break;
                    }
            }
        }
    }

    // Sets all the UI sprites to inactive
    private void DisableAllUI()
    {
        for (int i = 0; i < m_baseKeySprites.Length; i++)
        {
            m_baseKeySprites[i].SetActive(false);
        }
        for (int i = 0; i < m_waterKeySprites.Length; i++)
        {
            m_waterKeySprites[i].SetActive(false);
        }
        for (int i = 0; i < m_fireKeySprites.Length; i++)
        {
            m_fireKeySprites[i].SetActive(false);
        }
        for (int i = 0; i < m_sandKeySprites.Length; i++)
        {
            m_sandKeySprites[i].SetActive(false);
        }
    }
}
