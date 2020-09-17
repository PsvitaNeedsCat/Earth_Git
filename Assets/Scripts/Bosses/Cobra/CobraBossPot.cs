using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraBossPot : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            player.GetComponent<HealthComponent>().Health -= 1;
        }
    }
}
