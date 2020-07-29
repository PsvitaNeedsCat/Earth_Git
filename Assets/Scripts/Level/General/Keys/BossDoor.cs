using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [Tooltip("Won't load a scene if left blank")]
    [SerializeField] private string m_sceneToLoadUponUnlock = "";
    [SerializeField] private bool m_unlockOverride = false;

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player)
        {
            if (player.m_hasKey || m_unlockOverride)
            {
                // Unlock
                MessageBus.TriggerEvent(EMessageType.doorUnlocked);
                player.m_hasKey = false;
                Destroy(this.gameObject);

                // Load scene if there is one
                if (m_sceneToLoadUponUnlock != "")
                {
                    RoomManager.Instance.LoadScene(m_sceneToLoadUponUnlock);
                }
            }
            else
            {
                // Door is locked
                MessageBus.TriggerEvent(EMessageType.doorLocked);
            }
        }
    }
}
