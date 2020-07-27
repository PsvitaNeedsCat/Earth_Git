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

    [SerializeField] private EventSystem m_eventSystem;
    [SerializeField] private GameObject m_splashScreen = null;
    [SerializeField] private GameObject m_menuObject = null;

    private static MainMenu s_instance;
    private static bool s_splashSeen = false;

    private void Awake()
    {
        if (s_instance != null && s_instance != this) { Destroy(this.gameObject); }
        else { s_instance = this; }

        // Disable splash screen
        if (s_splashSeen)
        {
            m_splashScreen.SetActive(false);
            m_menuObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        m_playButton.DOKill();
        m_quitButton.DOKill();
    }

    // Called when play is pressed
    public void PressPlay()
    {
        s_splashSeen = true;

        // Deselect buttons
        m_eventSystem.SetSelectedGameObject(null);

        // Remove play and quit buttons - display save files
        FadeButton(m_playButton, false);
        FadeButton(m_quitButton, false);

        for (int i = 0; i < m_saveButtons.Length; i++)
        {
            // Update text
            m_saveButtons[i].GetComponent<SaveFileButton>().CheckSaveStatus();

            // Fade in
            FadeButton(m_saveButtons[i], true, (i == 0));

            // Fade in delete button
            if (m_deleteButtons[i].IsActive())
            {
                FadeButton(m_deleteButtons[i], true);
            }
        }

        // Return button
        FadeButton(m_returnButton, true);
    }

    // Fades a given button in or out
    private void FadeButton(Button _button, bool _fadeIn, bool _setFocus = false)
    {
        // Set alpha
        Color colour = _button.image.color;
        colour.a = (_fadeIn) ? 0.0f : 1.0f;
        _button.image.color = colour;

        // Fade in/out
        if (_fadeIn)
        {
            _button.gameObject.SetActive(true);
            if (_setFocus)
            { _button.image.DOFade(1.0f, 0.5f).OnComplete(() => m_eventSystem.SetSelectedGameObject(_button.gameObject)); }
            else
            { _button.image.DOFade(1.0f, 0.5f); }
        }
        else
        {
            _button.image.DOFade(0.0f, 0.5f).OnComplete(() => _button.gameObject.SetActive(false));
        }
    }

    // Going back to the main options from selecting a save
    public void ReturnFromSaves()
    {
        // Deselect buttons
        m_eventSystem.SetSelectedGameObject(null);

        // Remove save buttons - display play and quit
        for (int i = 0; i < m_saveButtons.Length; i++)
        {
            FadeButton(m_saveButtons[i], false);

            if (m_deleteButtons[i].IsActive())
            {
                FadeButton(m_deleteButtons[i], false);
            }
        }

        // Return button
        FadeButton(m_returnButton, false);

        // Activate play and quit buttons
        FadeButton(m_playButton, true, true);
        FadeButton(m_quitButton, true);
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

    // Sets the event system's focus to the given save button
    public void SetSaveButtonFocus(int _saveId)
    {
        m_eventSystem.SetSelectedGameObject(m_saveButtons[_saveId].gameObject);
    }

    // Updates the save file buttons
    public void UpdateSaves()
    {
        for (int i = 0; i < m_saveButtons.Length; i++)
        {
            m_saveButtons[i].GetComponent<SaveFileButton>().CheckSaveStatus();
        }
    }
}
