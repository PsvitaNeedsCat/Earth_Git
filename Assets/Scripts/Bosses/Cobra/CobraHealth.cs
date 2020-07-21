using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CobraHealth : MonoBehaviour
{
    public List<GameObject> m_healthIcons;
    private static List<GameObject> s_healthIcons;

    private static CobraStateSettings m_settingsFull;
    private static CobraStateSettings m_settingsHurtOnce;
    private static CobraStateSettings m_settingsHurtTwice;

    private static BoxCollider m_collider;
    private static int m_currentHealth;
    private static CobraHealth m_health;

    private static CobraMirageBarrage m_barrage;
    private static CobraBoss m_boss;

    public static CobraStateSettings StateSettings
    {
        get
        {
            switch (m_currentHealth)
            {
                case 3:
                {
                    return m_settingsFull;
                }

                case 2:
                {
                    return m_settingsHurtOnce;
                }
                
                case 1:
                {
                    return m_settingsHurtTwice;
                }

                default:
                {
                    return null;
                }
            }
               
        }
    }

    private void Awake()
    {
        m_settingsFull = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsFull");
        m_settingsHurtOnce = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsHurtOnce");
        m_settingsHurtTwice = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsHurtTwice");

        m_boss = GetComponent<CobraBoss>();
        m_collider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        // If there is already a cobra health, destroy this
        if (m_health != null)
        {
            Destroy(transform.parent.parent.gameObject);
        }
        // Otherwise, assign this as the instance
        else
        {
            m_health = this;
        }

        s_healthIcons = m_healthIcons;
        m_barrage = GetComponent<CobraMirageBarrage>();
    }

    private void OnDisable()
    {
        m_health = null;
        s_healthIcons.Clear();
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

    public static void Damage()
    {
        if (m_currentHealth == 0)
        {
            return;
        }

        m_currentHealth -= 1;
        s_healthIcons[0].transform.parent.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        s_healthIcons[m_currentHealth].SetActive(false);

        m_barrage.CancelAttack();

        if (m_currentHealth == 0)
        {
            m_boss.StartChase();
        }
    }
}
