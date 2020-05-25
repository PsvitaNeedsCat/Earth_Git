using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player)
        {
            if (player.m_hasKey)
            {
                // Unlock
                MessageBus.TriggerEvent(EMessageType.doorUnlocked);
                player.m_hasKey = false;
                Destroy(this.gameObject);
            }
            else
            {
                // Door is locked
                MessageBus.TriggerEvent(EMessageType.doorLocked);
            }
        }
    }
}
