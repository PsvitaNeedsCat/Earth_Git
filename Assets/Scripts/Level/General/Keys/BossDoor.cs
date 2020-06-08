using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [Tooltip("Won't load a scene if left blank")]
    public string m_sceneToLoadUponUnlock = "";

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

                // Temp
                if (m_sceneToLoadUponUnlock != "") UnityEngine.SceneManagement.SceneManager.LoadScene(m_sceneToLoadUponUnlock);
            }
            else
            {
                // Door is locked
                MessageBus.TriggerEvent(EMessageType.doorLocked);
            }
        }
    }
}
