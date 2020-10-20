using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    public List<Image> m_healthImages;
    public List<Image> m_healthBackgroundImages;

    public GameObject m_blackwall = null;

    private void Awake()
    {
        if (!m_blackwall.activeSelf)
        {
            m_blackwall.SetActive(true);
        }
    }
}
