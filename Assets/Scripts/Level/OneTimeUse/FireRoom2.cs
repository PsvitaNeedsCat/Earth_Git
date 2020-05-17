using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRoom2 : MonoBehaviour
{
    private int m_grubTotal = 3;
    private int m_currentGrubs = 3;

    [SerializeField] GameObject m_keyPrefab;
    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.grubKilled, GrubKilled);
    }

    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.grubKilled, GrubKilled);
    }

    private void GrubKilled(string _null)
    {
        m_currentGrubs -= 1;
        if (m_currentGrubs == 0)
        {
            // Spawn key
            MessageBus.TriggerEvent(EMessageType.keySpawned);
            Instantiate(m_keyPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
