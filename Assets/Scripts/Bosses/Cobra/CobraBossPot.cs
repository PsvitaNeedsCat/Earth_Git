using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraBossPot : MonoBehaviour
{
    public CobraPot m_bossPot;

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player && m_bossPot.m_damagePlayer)
        {
            player.GetComponent<HealthComponent>().Health -= 1;
        }
    }
}
