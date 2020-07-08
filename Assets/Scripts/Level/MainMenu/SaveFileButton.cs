﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class SaveFileButton : MonoBehaviour
{
    [SerializeField] private int m_saveId;

    MainMenu m_menuManager;

    public void TryLoadSave() => m_menuManager.TryLoadSave(m_saveId);

    private void Awake()
    {
        m_menuManager = FindObjectOfType<MainMenu>();
    }

    // Checks that status of the save file and updates the text
    public void CheckSaveStatus()
    {
        SaveManager.SaveFile save = SaveManager.Instance.GetSaveFile(m_saveId);

        string text = "Empty";

        // If there is a save
        if (save != null)
        {
            text = save.scene;
        }

        GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
