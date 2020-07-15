using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraHealth : MonoBehaviour
{
    private static CobraStateSettings m_settingsFull;
    private static CobraStateSettings m_settingsHurtOnce;
    private static CobraStateSettings m_settingsHurtTwice;

    private static int m_currentHealth;

    public static CobraStateSettings StateSettings
    {
        get
        {
            switch (m_currentHealth)
            {
                case 3: return m_settingsFull;
                case 2: return m_settingsHurtOnce;
                case 1: return m_settingsHurtTwice;
                default: return null;
            }
               
        }
    }

    private void Awake()
    {
        m_currentHealth = CobraBoss.m_settings.m_maxHealth;

        m_settingsFull = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsFull");
        m_settingsHurtOnce = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsHurtOnce");
        m_settingsHurtTwice = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsHurtTwice");
    }
}
