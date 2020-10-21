using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CobraWakeup : MonoBehaviour
{
    [SerializeField] private CobraBoss m_boss;
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
            m_boss.TriggerWakeup();
            m_bossWall.SetActive(true);
            m_director.Play();
            m_hasTriggered = true;
            MessageBus.TriggerEvent(EMessageType.dCobraMusic);
        }
    }
}
