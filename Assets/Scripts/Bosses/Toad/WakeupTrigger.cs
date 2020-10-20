using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WakeupTrigger : MonoBehaviour
{
    [SerializeField] private ToadBoss m_boss;
    [SerializeField] private GameObject m_bossWall;
    [SerializeField] private PlayableDirector m_director;
    private bool m_hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (m_hasTriggered)
        {
            return;
        }

        if (other.GetComponent<Player>())
        {
            m_boss.TriggerToadWakeup();
            m_bossWall.SetActive(true);
            m_director.Play();
            MessageBus.TriggerEvent(EMessageType.wToadMusic);
            m_hasTriggered = true;
        }
    }
}
