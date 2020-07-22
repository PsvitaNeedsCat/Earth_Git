using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overworld : MonoBehaviour
{
    private static Overworld s_instance;

    private bool[] m_unlockedLevels = new bool[3]
    {
        true,
        false,
        false
    };

    private void Awake()
    {
        // Only one instance
        if (s_instance != null && s_instance != this) { Destroy(this.gameObject); }
        else { s_instance = this; }
    }

    // Updates and checks if the level is unlocked based off of what powers the player has unlocked
    public bool IsLevelUnlocked(int _id)
    {
        // Update unlocked levels
        for (int i = 0; i < m_unlockedLevels.Length; i++)
        {
            m_unlockedLevels[i] = Player.s_activePowers[(EChunkEffect)i];
        }

        return m_unlockedLevels[_id];
    }
}
