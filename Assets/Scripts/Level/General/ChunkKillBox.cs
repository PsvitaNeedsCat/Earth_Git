using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkKillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk)
        {
            Destroy(chunk.gameObject);
        }
    }
}
