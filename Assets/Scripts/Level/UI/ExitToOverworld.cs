using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.GameCenter;

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
            centredPos.x = 0.0f;
            m_viewControlsButton.anchoredPosition = centredPos;
        }
    }
}
