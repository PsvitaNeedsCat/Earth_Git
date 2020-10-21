using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class Screenshot : MonoBehaviour
{

    public int sizeMultiplier = 1;
    public string screenshotName = "screenshot";
    public string fileType = ".png";

    private int screenshotNumber = 0;

    private void Awake()
    {
        screenshotNumber = EditorPrefs.GetInt("screenshotNumber", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot(screenshotName + screenshotNumber.ToString() + fileType, sizeMultiplier);
            screenshotNumber++;
            EditorPrefs.SetInt("screenshotNumber", screenshotNumber);
            Debug.Log("Screenshot Taken!");
        }
    }
}
