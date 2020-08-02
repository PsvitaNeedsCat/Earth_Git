using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SandManager : MonoBehaviour
{
    [SerializeField] private Tile[] m_spawnTiles;
    [SerializeField] private float m_spawnHeight = 10.0f;
    [SerializeField] private GameObject m_sandPrefab;

    // Called by button
    public void ResetSand()
    {
        SandBlock[] m_sandBlocks = FindObjectsOfType<SandBlock>();

        if (m_sandBlocks.Length > 0)
        {
            MessageBus.TriggerEvent(EMessageType.sandDestroyed);
        }

        List<int> ignoreTiles = new List<int>();

        // Destroy all sand
        for (int i = m_sandBlocks.Length - 1; i >= 0; i--)
        {
            if (m_sandBlocks[i].IsGrounded())
            {
                Destroy(m_sandBlocks[i].gameObject);
            }
            else
            {
                ignoreTiles.Add(i);
            }
        }

        // Spawn new tiles
        for (int i = 0; i < m_spawnTiles.Length; i++)
        {
            // Ignore tiles for sand that is in the air
            if (ignoreTiles.Contains(i))
            {
                continue;
            }

            // Spawn new one
            GameObject newSand = Instantiate(m_sandPrefab, m_spawnTiles[i].transform.position, Quaternion.identity); // Spawn
            newSand.transform.parent = this.transform; // Set parent
            newSand.transform.position = newSand.transform.position + new Vector3(0.0f, m_spawnHeight, 0.0f); // Add height
            newSand.GetComponent<SandBlock>().Fall(); // Fall
        }
    }
}
