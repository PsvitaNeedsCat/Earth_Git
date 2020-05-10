using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadWaveSegment : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HealthComponent healthComp = other.GetComponent<HealthComponent>();

        if (healthComp && other.GetComponent<PlayerController>())
        {
            healthComp.Health -= 1;
        }

        this.gameObject.SetActive(false);
    }
}
