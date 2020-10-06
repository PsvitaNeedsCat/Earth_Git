using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private GameObject m_title = null;

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
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            s_instance = this; 
        }

        // Disable splash screen
        if (s_splashSeen)
        {
            m_splashScreen.SetActive(false);
            m_menuObject.SetActive(true);
        }

        Cursor.visible = false;
#if UNITY_EDITOR
        Cursor.visible = true;
#endif
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

        // Tween out title
        Vector3 newPos = m_title.transform.position;
        newPos.x -= 12.0f;
        m_title.transform.DOMove(newPos, 0.5f);

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
        if (_fadeIn)
        {
            _button.gameObject.SetActive(true);
        }

        // Fade in/out image
        if (_button.image)
        {
            Color colour = _button.image.color;
            colour.a = (_fadeIn) ? 0.0f : 1.0f;
            _button.image.color = colour;

            // Fade in/out
            _button.image.DOFade((_fadeIn) ? 1.0f : 0.0f, 0.5f);
        }

        // Fade in/out text
        if (_button.transform.childCount > 0)
        {
            TextMeshProUGUI tmpro = _button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpro)
            {
                Color newColour = tmpro.color;
                newColour.a = (_fadeIn) ? 1.0f : 0.0f;
                tmpro.DOColor(newColour, 0.5f);
            }
        }

        // Set focus
        StartCoroutine(TimedFadeInOut(_fadeIn,_setFocus, _button));
    }

    private IEnumerator TimedFadeInOut(bool _fadeIn, bool _setFocus, Button _button)
    {
        yield return new WaitForSeconds(0.5f);

        if (_setFocus)
        {
            m_eventSystem.SetSelectedGameObject(_button.gameObject);
        }

        if (!_fadeIn)
        {
            _button.gameObject.SetActive(false);
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

        // Fade in title
        Vector3 newPos = m_title.transform.position;
        newPos.x += 12.0f;
        m_title.transform.DOMove(newPos, 0.5f);
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
