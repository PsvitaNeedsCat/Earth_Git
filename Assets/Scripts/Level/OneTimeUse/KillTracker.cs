using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTracker : MonoBehaviour
{
    [SerializeField] private EMessageType m_killMessage = EMessageType.none;
    [SerializeField] private int m_enemyTotal = 0;

    private int m_enemyCount = 0;

    private GameObject m_keyPrefab;

    private void Awake()
    {
        m_keyPrefab = Resources.Load<GameObject>("Prefabs/Key");
        m_enemyCount = m_enemyTotal;
    }

    private void OnEnable()
    {
        MessageBus.AddListener(m_killMessage, EnemyKilled);
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(m_killMessage, EnemyKilled);
    }

    private void EnemyKilled(string _null)
    {
        m_enemyCount -= 1;
        if (m_enemyCount == 0)
        {
            // Spawn key
            MessageBus.TriggerEvent(EMessageType.keySpawned);
            Instantiate(m_keyPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
