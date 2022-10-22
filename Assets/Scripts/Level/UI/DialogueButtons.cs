using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButtons : MonoBehaviour
{
    public enum EButton
    {
        A,
        Rock,
        Water,
        Fire,
    };

    [SerializeField] private GameObject[] m_buttons = new GameObject[] { };

    public void ChangeActiveButton(EButton _button)
    {
        foreach (GameObject button in m_buttons)
        {
            button.SetActive(false);
        }

        if (_button == EButton.A)
        {
            m_buttons[0].SetActive(true);
            return;
        }

        m_buttons[1].SetActive(true);
    }
}
