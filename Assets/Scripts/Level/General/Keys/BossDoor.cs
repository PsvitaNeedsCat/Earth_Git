using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [Tooltip("Won't load a scene if left blank")]
    [SerializeField] private string m_sceneToLoadUponUnlock = "";

    private void OnCollisionEnter(Collision collision)
    {
        PlayerInput player = collision.collider.GetComponent<PlayerInput>();
        if (player)
        {
            if (player.GetComponent<Player>().m_hasKey)
            {
                // Unlock
                MessageBus.TriggerEvent(EMessageType.doorUnlocked);
                Destroy(this.gameObject);

                // Temp
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
