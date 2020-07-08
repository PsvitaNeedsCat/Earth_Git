using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private Button[] m_saveButtons = new Button[3];
    [SerializeField] private Button[] m_deleteButtons = new Button[3];
    [SerializeField] private Button m_returnButton;

    private EventSystem m_eventSystem;

    private void Awake()
    {
        m_eventSystem = FindObjectOfType<EventSystem>();
    }

    private void OnDisable()
    {
        m_playButton.DOKill();
        m_quitButton.DOKill();
    }

    // Called when play is pressed
    public void PressPlay()
    {
        // Deselect buttons
        m_eventSystem.SetSelectedGameObject(null);

        // Remove play and quit buttons - display save files
        m_playButton.image.DOFade(0.0f, 0.5f).OnComplete(() => m_playButton.gameObject.SetActive(false));
        m_quitButton.image.DOFade(0.0f, 0.5f).OnComplete(() => m_quitButton.gameObject.SetActive(false));

        foreach (Button b in m_saveButtons)
        {
            // Set alpha to zero
            Color newColour = b.image.color;
            newColour.a = 0.0f;
            b.image.color = newColour;

            // Activate object
            b.gameObject.SetActive(true);

            // Update text
            b.GetComponent<SaveFileButton>().CheckSaveStatus();

            // Fade in
            b.image.DOFade(1.0f, 0.5f);

            //if (m_deleteButtons)
        }

        // Return button
        Color colour = m_returnButton.image.color;
        colour.a = 0.0f;
        m_returnButton.image.color = colour;
        m_returnButton.gameObject.SetActive(true);
        m_returnButton.image.DOFade(1.0f, 0.5f);

        // Set the event system
        m_eventSystem.SetSelectedGameObject(m_saveButtons[0].gameObject);
    }

    // Going back to the main options from selecting a save
    public void ReturnFromSaves()
    {
        // Deselect buttons
        m_eventSystem.SetSelectedGameObject(null);

        // Remove save buttons - display play and quit
        foreach (Button b in m_saveButtons)
        {
            b.image.DOFade(0.0f, 0.5f).OnComplete(() => b.gameObject.SetActive(false));
        }

        // Return button
        m_returnButton.image.DOFade(0.0f, 0.5f).OnComplete(() => m_returnButton.gameObject.SetActive(false));

        // Activate play and quit buttons
        Color playColour = m_playButton.image.color;
        Color quitColour = m_quitButton.image.color;
        playColour.a = 0.0f;
        quitColour.a = 0.0f;
        m_playButton.gameObject.SetActive(true);
        m_quitButton.gameObject.SetActive(true);
        m_playButton.image.DOFade(1.0f, 0.5f).OnComplete(() => m_eventSystem.SetSelectedGameObject(m_playButton.gameObject));
        m_quitButton.image.DOFade(1.0f, 0.5f);
    }

    // Tries to load a save, otherwise it will create a save
    public void TryLoadSave(int _saveId)
    {
        // If save does not exist or is invalid
        if (!SaveManager.Instance.LoadGame(_saveId))
        {
            SaveManager.Instance.CreateSave(_saveId);
        }
    }
}
