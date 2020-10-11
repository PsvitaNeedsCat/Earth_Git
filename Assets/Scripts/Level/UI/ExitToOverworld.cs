using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ExitToOverworld : MonoBehaviour
{
    [SerializeField] private RectTransform m_viewControlsButton = null;

    private void Awake()
    {
        string curSceneName = SceneManager.GetActiveScene().name;
        if (curSceneName == "Overworld" || curSceneName == "Dojo")
        {
            gameObject.SetActive(false);
            Vector3 centredPos = m_viewControlsButton.anchoredPosition;
            centredPos.y = 0.0f;
            m_viewControlsButton.anchoredPosition = centredPos;
        }
    }
}
