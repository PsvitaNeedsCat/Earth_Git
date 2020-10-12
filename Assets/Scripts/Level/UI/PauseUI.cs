using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private EChunkEffect m_effect = EChunkEffect.mirage;
    private static PauseUI s_instance = null;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }

        s_instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(!Player.s_activePowers[m_effect]);
    }
}
