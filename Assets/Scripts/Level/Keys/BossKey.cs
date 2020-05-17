using UnityEngine;

public class BossKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            // Collect key
            MessageBus.TriggerEvent(EMessageType.keyCollected);
            player.m_hasKey = true;
            Destroy(this.gameObject);
        }
    }
}
