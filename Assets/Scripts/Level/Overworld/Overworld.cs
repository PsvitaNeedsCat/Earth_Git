using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class Overworld : MonoBehaviour
{
    private static Overworld s_instance;

    [SerializeField] private TeleportToHere[] m_spawns = new TeleportToHere[] { };
    
    [SerializeField] private UnityEvent[] m_startupEvents = new UnityEvent[4];

    // Keeps an eye on m_startupEvents so that it stays at the size 4
    private void OnValidate()
    {
        if (m_startupEvents.Length != 4)
        {
            Debug.LogWarning("Do not change the size of Startup Events");
            System.Array.Resize(ref m_startupEvents, 4);
        }
    }

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
    }

    private void Start()
    {
        // Check player's last entered temple - decides spawn
        m_spawns[Player.m_lastTempleEntered].Teleport();

        // Call a different event depending on how many powers the player has unlocked
        int numPowersUnlocked = -1;
        foreach (KeyValuePair<EChunkEffect, bool> i in Player.s_activePowers)
        {
            if (i.Value)
            {
                ++numPowersUnlocked;
            }
        }

        // Only call startup event if the player has come from another temple
        if (Player.m_lastTempleEntered != (numPowersUnlocked + 1))
        {
            m_startupEvents[numPowersUnlocked].Invoke();
        }
    }
}
