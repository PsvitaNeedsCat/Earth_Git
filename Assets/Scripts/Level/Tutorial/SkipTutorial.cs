using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;

public class SkipTutorial : MonoBehaviour
{
    [SerializeField] private Image m_skipButtonPrompt = null;

    private float m_skipTimer = 0.0f;
    private float m_heldButtonMin = 1.5f;
    private bool m_skipButtonHeld = false;

    private void Start()
    {
        PlayerInput.s_controls.Tutorial.Enable();

        PlayerInput.s_controls.Tutorial.SkipTutorial.performed += _ => m_skipButtonHeld = true;
        PlayerInput.s_controls.Tutorial.SkipTutorial.canceled += _ => m_skipButtonHeld = false;
    }

    private void OnDestroy()
    {
        m_skipButtonHeld = false;
        m_skipTimer = 0.0f;
        PlayerInput.s_controls.Tutorial.Disable();
    }

    // If the player has held the skip button for a specified time, it'll skip the tutorial
    private void Update()
    {
        m_skipTimer += (m_skipButtonHeld) ? Time.deltaTime : -1.0f * Time.deltaTime;

        if (m_skipTimer < 0.0f)
        {
            m_skipTimer = 0.0f;
        }
        else if (m_skipTimer >= m_heldButtonMin)
        {
            LoadOverworld();
        }

        m_skipButtonPrompt.fillAmount = (m_skipTimer == 0.0f) ? 0.0f : m_skipTimer / m_heldButtonMin;
    }

    // Skips the tutorial by loading the next scene
    private void LoadOverworld()
    {
        m_skipButtonPrompt.rectTransform.DOScale(Vector3.one * 1.4f, 0.5f);
        m_skipButtonPrompt.DOFade(0.0f, 0.5f);

        RoomManager.Instance.LoadScene("Overworld");

        Destroy(this);
    }
}
