using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraHealth : MonoBehaviour
{
    private static BoxCollider m_collider;

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
        m_settingsFull = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsFull");
        m_settingsHurtOnce = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsHurtOnce");
        m_settingsHurtTwice = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsHurtTwice");

        m_collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            m_currentHealth--;
        }
    }

    public void SetCurrentHealth(int _newHealth)
    {
        m_currentHealth = _newHealth;
    }

    public static void SetCollider(bool _active)
    {
        m_collider.enabled = _active;
    }

    public static void OnHit()
    {
        m_currentHealth -= 1;
    }
}
