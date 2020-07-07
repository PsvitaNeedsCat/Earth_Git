﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Overworld");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        FindObjectOfType<PlayerController>().UnPause();
    }
}
