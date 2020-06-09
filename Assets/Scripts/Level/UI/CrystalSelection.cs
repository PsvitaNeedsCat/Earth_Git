using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class CrystalSelection : MonoBehaviour
{
    [SerializeField] private Sprite[] m_backgroundSprites;
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private Image[] m_gemSprites;

    public void UpdateSelected(int _selected)
    {
        m_backgroundImage.sprite = m_backgroundSprites[_selected];
    }

    public void UpdateUnlocked(bool[] _active)
    {
        // Enable the images when they are unlocked
        for (int i = 0; i < m_gemSprites.Length; i++)
        {
            m_gemSprites[i].enabled = _active[i];
        }
    }
}
