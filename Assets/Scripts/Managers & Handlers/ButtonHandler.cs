using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

    public void SaveAndQuit()
    {
        SaveManager.Instance.SaveGame();

        Time.timeScale = 1.0f;

        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        if (mainMenu) { Destroy(mainMenu.gameObject); }

        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        FindObjectOfType<PlayerController>().UnPause();
    }

    public void DeleteSave(int _saveId)
    {
        // Delete the save
        SaveManager.Instance.DeleteSave(_saveId);

        MainMenu menuScript = FindObjectOfType<MainMenu>();
        Debug.Assert(menuScript, "Button handler could not find MainMenu script");

        // Update the button displays
        menuScript.UpdateSaves();

        // Change the focus
        menuScript.SetSaveButtonFocus(_saveId);
    }

    public void ReturnToOverworld()
    {
        Resume();
        RoomManager.Instance.LoadScene("Overworld");
    }
}
