using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player && player.m_hasKey)
        {
            // Unlock
            player.m_hasKey = false;
            Destroy(this.gameObject);
        }
    }
}
