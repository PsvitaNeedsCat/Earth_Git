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
    private EButton m_activeButton = EButton.A;

    public void ChangeActiveButton(EButton _button)
    {
        m_buttons[(int)m_activeButton].SetActive(false);

        m_buttons[(int)_button].SetActive(true);

        m_activeButton = _button;
    }
}
