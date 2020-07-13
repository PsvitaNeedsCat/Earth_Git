using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        CheckForPlayer(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckForPlayer(collision);
    }

    private void CheckForPlayer(Collision _collision)
    {
        // If hit player
        PlayerController player = _collision.collider.GetComponent<PlayerController>();
        if (player &&
            player.transform.position.y > transform.position.y) // Player is above stairs
        {
            player.CancelStairsGravity(transform.right);
        }
    }
}
