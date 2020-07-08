using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeHead : MonoBehaviour
{
    // Damage things when hit by the centipede's head
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player)
        {
            // Calculate knockback on the player
            Vector3 dir = (player.transform.position - transform.position);
            dir.y = 0.0f;
            player.KnockBack(dir.normalized);
            player.GetComponent<HealthComponent>().Health -= 1;

            return;
        }

        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk)
        {
            chunk.GetComponent<HealthComponent>().Health = 0;
            MessageBus.TriggerEvent(EMessageType.chunkDestroyed);
        }
    }
}
