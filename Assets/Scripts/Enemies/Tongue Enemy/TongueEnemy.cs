using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueEnemy : MonoBehaviour
{
    public enum State
    {
        idle,
        extending,
        retracting
    }
    public State m_state = State.idle;

    [SerializeField] private Tongue m_tongue;
    [SerializeField] private bool m_autoStart = true;
    private bool m_activated = false;

    private GlobalEnemySettings m_settings;
    private float m_tongueTimer = 0.0f;
    private float m_retractingTimer = 0.0f;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        m_tongueTimer = m_settings.m_TongueCooldown;

        m_activated = m_autoStart;
    }

    private void OnEnable()
    {
        // Reset animation & state
        m_state = State.idle;
        m_tongueTimer = 0.0f;
        m_retractingTimer = 0.0f;
    }

    private void Update()
    {
        if (!m_activated)
        {
            return;
        }

        if (m_state == State.idle)
        {
            m_tongueTimer -= Time.deltaTime;
            if (m_tongueTimer <= 0.0f)
            {
                m_state = State.extending;

                m_tongueTimer = m_settings.m_TongueCooldown;
                m_retractingTimer = 1.0f;
                
                m_tongue.gameObject.SetActive(true);
                m_tongue.Extend();
            }
        }
        else if (m_state == State.retracting)
        {
            m_retractingTimer -= Time.deltaTime;
            if (m_retractingTimer <= 0.0f)
            {
                Swallow();
            }
        }
    }

    public void Swallow()
    {
        m_state = State.idle;

        m_tongue.Swallow();

        EChunkType typeSwallowed = m_tongue.GetAttached();

        if (typeSwallowed == EChunkType.poison)
        {
            Dead();
        }

        if (typeSwallowed != EChunkType.none)
        {
            MessageBus.TriggerEvent(EMessageType.enemySwallow); 
        }

        m_tongue.gameObject.SetActive(false);
    }

    // Kills the enemy - called when swallowed poison chunk
    private void Dead()
    {
        MessageBus.TriggerEvent(EMessageType.tongueEnemyKilled);

        Destroy(gameObject);

        EffectsManager.SpawnEffect(EffectsManager.EEffectType.statueBreak, transform.position, Quaternion.identity, Vector3.one, 1.0f);
    }

    // Begins the toad's motion - sets the timer to 0 so it immediately extends its tongue
    public void ActivateToad()
    {
        if (m_activated)
        {
            return;
        }

        m_tongueTimer = 0.0f;

        m_activated = true;
    }
}
