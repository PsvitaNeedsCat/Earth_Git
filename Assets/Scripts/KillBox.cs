using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HealthComponent healthComp = other.GetComponent<HealthComponent>();

        if (healthComp && healthComp.m_type == HealthComponent.EHealthType.player)
        {
            healthComp.Health = 0;
        }
    }
}
