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

    private GlobalEnemySettings m_settings;
    private float m_tongueTimer = 0.0f;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        m_tongueTimer = m_settings.m_TongueCooldown;
    }

    private void Update()
    {
        if (m_state == State.idle)
        {
            m_tongueTimer -= Time.deltaTime;
            if (m_tongueTimer <= 0.0f)
            {
                m_state = State.extending;

                m_tongueTimer = m_settings.m_TongueCooldown;
                
                m_tongue.gameObject.SetActive(true);
                m_tongue.Extend();
            }
        }
    }

    public void Swallow()
    {
        m_state = State.idle;

        eChunkType typeSwallowed = m_tongue.GetAttached();

        if (typeSwallowed == eChunkType.poison) { Destroy(this.gameObject); }

        m_tongue.gameObject.SetActive(false);
    }
}
