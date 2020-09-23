using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Manages the health of the cobra boss, and how its attack settings change at different health values
public class CobraHealth : MonoBehaviour
{
    public List<GameObject> m_healthIcons;
    public SkinnedMeshRenderer m_meshRenderer;
    
    private static List<GameObject> s_healthIcons;

    private static CobraStateSettings s_settingsFull;
    private static CobraStateSettings s_settingsHurtOnce;
    private static CobraStateSettings s_settingsHurtTwice;

    private static BoxCollider s_collider;
    private static int s_currentHealth;
    private static CobraHealth s_health;

    private static CobraMirageBarrage s_barrage;
    private static CobraBoss s_boss;
    private static CobraShuffle s_shuffle;
    private static CobraAnimations s_animations;

    private static Material s_material;

    // Return the appropriate settings variable based on what health we are on
    public static CobraStateSettings StateSettings
    {
        get
        {
            switch (s_currentHealth)
            {
                case 3:
                {
                    return s_settingsFull;
                }

                case 2:
                {
                    return s_settingsHurtOnce;
                }
                
                case 1:
                {
                    return s_settingsHurtTwice;
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
        s_health = this;
         
        s_healthIcons = m_healthIcons;
        s_barrage = GetComponent<CobraMirageBarrage>();
        s_shuffle = GetComponent<CobraShuffle>();
        s_animations = GetComponent<CobraAnimations>();

        // Initialise variables
        s_settingsFull = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsFull");
        s_settingsHurtOnce = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsHurtOnce");
        s_settingsHurtTwice = Resources.Load<CobraStateSettings>("ScriptableObjects/CobraBossSettingsHurtTwice");

        s_boss = GetComponent<CobraBoss>();
        s_collider = GetComponent<BoxCollider>();

        m_meshRenderer.material = new Material(m_meshRenderer.material);
        s_material = m_meshRenderer.material;
    }

    private void Start()
    {
        s_currentHealth = CobraBoss.s_settings.m_maxHealth;
    }

    private void OnDestroy()
    {
        // s_health = null;
        // s_healthIcons.Clear();
    }

    private void Update()
    {
    }

    public void SetCurrentHealth(int _newHealth)
    {
        s_currentHealth = _newHealth;
    }

    //public static void SetCollider(bool _active)
    //{
    //    s_collider.enabled = _active;
    //}

    public static void Damage()
    {
        if (s_currentHealth == 0)
        {
            return;
        }

        // Decrement health value and update health UI
        s_currentHealth -= 1;
        s_healthIcons[0].transform.parent.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        s_healthIcons[s_currentHealth].SetActive(false);

        s_health.StartCoroutine(s_health.OnDamaged());
    }

    private IEnumerator OnDamaged()
    {
        MessageBus.TriggerEvent(EMessageType.cobraDamaged);

        s_animations.Damaged();
        Debug.Log("Set cobra damaged trigger");

        s_barrage.CancelAttack();

        s_material.SetFloat("_FresnelStrength", 5.0f);
        s_material.SetFloat("_Cutoff", 0.8f);

        yield return new WaitForSeconds(1.0f);

        // If on 0 health, start the chase behaviour
        if (s_currentHealth == 0)
        {
            s_boss.StartChase();
        }
        else
        {
            // Boss chooses a new pot
            List<int> possiblePositions = StateSettings.m_barrageAttackPositions;
            int newBossPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            CobraPot bossPot = s_health.GetComponent<CobraPot>();
            CobraPot swapPot = s_boss.m_cobraPots[newBossPosition];

            SwapPots(bossPot, bossPot.gameObject.transform.parent, swapPot, swapPot.transform);

            // s_boss.gameObject.transform.parent.position = CobraShuffle.s_potStartingPositions[newBossPosition];
            // s_boss.gameObject.transform.parent.rotation = CobraShuffle.s_potStartingRotations[newBossPosition];

            s_boss.SortPotList();

            CobraShuffle.s_bossPotIndex = newBossPosition;
        }
    }

    private static void SwapPots(CobraPot _potOne, Transform _potOneTransform, CobraPot _potTwo, Transform _potTwoTransform)
    {
        // Store pot one variables
        int potOneIndex = _potOne.m_potIndex;
        Vector3 potOnePosition = _potOneTransform.position;
        Quaternion potOneRotation = _potOneTransform.rotation;

        // Copy pot two variables onto pot one
        _potOne.m_potIndex = _potTwo.m_potIndex;
        _potOneTransform.position = _potTwoTransform.position;
        _potOneTransform.rotation = _potTwoTransform.rotation;

        // Copy stored pot one variables onto pot two
        _potTwo.m_potIndex = potOneIndex;
        _potTwoTransform.position = potOnePosition;
        _potTwoTransform.rotation = potOneRotation;
    }

    public static int GetCurrentHealth()
    {
        return s_currentHealth;
    }
}
