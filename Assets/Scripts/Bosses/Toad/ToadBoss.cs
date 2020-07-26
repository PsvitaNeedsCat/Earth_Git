using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToadBoss : MonoBehaviour
{
    public Animator m_toadAnimator;
    public List<ToadBehaviour> m_behaviourLoop;

    public List<GameObject> m_healthIcons;

    public static EChunkType s_eaten = EChunkType.none;
    public static bool s_tookDamage = false;

    private int m_currentBehaviourIndex = 0;
    private int m_totalBehaviours;
    private ToadBehaviour m_currentBehaviour;
    private HealthComponent m_healthComp;
    private bool m_didSpitAttack = false;
    
    ToadBossSettings m_toadSettings;

    [SerializeField] private GameObject m_crystal;
    public void ActivateCrystal() => m_crystal.SetActive(true);

    private void Awake()
    {
        m_totalBehaviours = m_behaviourLoop.Count;
        m_toadSettings = Resources.Load<ToadBossSettings>("ScriptableObjects/ToadBossSettings");
        m_healthComp = GetComponent<HealthComponent>();
        m_healthComp.Init(m_toadSettings.m_maxHealth, m_toadSettings.m_maxHealth, DamageTaken, null, Died);
        s_tookDamage = false;
        s_eaten = EChunkType.none;
    }

    private void Start()
    {
        m_currentBehaviour = m_behaviourLoop[0];

        StartCoroutine(AwakenAfter(m_toadSettings.m_wakeAfter));
    }

    private void Update()
    {
        UpdateBehaviour();
    }

    private void UpdateBehaviour()
    {
        if (m_currentBehaviour.m_currentState == ToadBehaviour.EBehaviourState.complete)
        {
            GoToNextBehaviour();
        }
    }

    private void GoToNextBehaviour()
    {
        m_currentBehaviour.Reset();
        m_currentBehaviourIndex = (m_currentBehaviourIndex + 1) % m_totalBehaviours;
        m_currentBehaviour = m_behaviourLoop[m_currentBehaviourIndex];

        CheckBehaviourSkips();

        m_currentBehaviour.StartBehaviour();
    }

    private void CheckBehaviourSkips()
    {
        // If we are about to do the spit attack, but we ate a poison block, skip to swell up
        if (m_currentBehaviour is ToadSpit)
        {
            if (s_eaten == EChunkType.poison)
            {
                s_eaten = EChunkType.none;
                m_currentBehaviourIndex = (m_currentBehaviourIndex + 1) % m_totalBehaviours;
                m_currentBehaviour = m_behaviourLoop[m_currentBehaviourIndex];
            }
            else
            {
                m_didSpitAttack = true;
            }
        }
        // If we've just done spit attack, skip swell up
        else if (m_currentBehaviour is ToadSwell && m_didSpitAttack)
        {
            m_didSpitAttack = false;
            m_currentBehaviourIndex = (m_currentBehaviourIndex + 1) % m_totalBehaviours;
            m_currentBehaviour = m_behaviourLoop[m_currentBehaviourIndex];
        }
    }

    private IEnumerator AwakenAfter(float _afterSeconds)
    {
        yield return new WaitForSeconds(_afterSeconds);

        m_toadAnimator.SetTrigger("Awaken");
    }

    public void AEAwaken()
    {
        m_currentBehaviour.StartBehaviour();
    }

    public void OnHit()
    {
        if (s_tookDamage || !(m_currentBehaviour is ToadSwell)) return;

        s_tookDamage = true;

        m_healthComp.Health -= 1;
    }

    private void DamageTaken()
    {
        // Play sound
        MessageBus.TriggerEvent(EMessageType.toadDamaged);

        // Update canvas
        m_healthIcons[0].transform.parent.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        m_healthIcons[m_healthComp.Health].SetActive(false);

        HitFreezeManager.BeginHitFreeze(0.1f);
    }

    private void Died()
    {
        // Turn canvas off

        m_toadAnimator.SetTrigger("Dead");

        // Remove script
        Destroy(this);
    }
}
