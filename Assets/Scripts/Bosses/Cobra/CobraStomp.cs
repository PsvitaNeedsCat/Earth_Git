using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraStomp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player)
        {
            player.GetComponent<HealthComponent>().Health -= 1;
        }
    }
}
