using UnityEngine;

public class BossKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            // Collect key
            player.m_hasKey = true;
            Destroy(this.gameObject);
        }
    }
}
