using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class HealingCrystal : Interactable
{
    // Heals the player
    override public void Invoke()
    {
        // Get player
        Player player = FindObjectOfType<Player>();
        Debug.Assert(player, "Could not find player to heal");

        // Get health component
        HealthComponent playerHealth = player.GetComponent<HealthComponent>();
        Debug.Assert(playerHealth, "Player doesn't have a health component");

        // Return to maximum health
        playerHealth.HealToMax();

        // Play sound
        MessageBus.TriggerEvent(EMessageType.crystalHealed);

        // Tween
        transform.DORewind();
        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }
}
