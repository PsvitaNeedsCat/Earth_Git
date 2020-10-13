using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private EChunkEffect m_effect = EChunkEffect.mirage;
    [SerializeField] private UnityEvent m_powerUnlockedEvent = new UnityEvent();
    [SerializeField] private UnityEvent m_powerLockedEvent = new UnityEvent();
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

        if (Player.s_activePowers[m_effect])
        {
            m_powerUnlockedEvent.Invoke();
        }
        else
        {
            m_powerLockedEvent.Invoke();
        }
    }
}
