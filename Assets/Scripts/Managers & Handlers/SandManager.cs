using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandManager : MonoBehaviour
{
    [HideInInspector] public List<GameObject> m_sandBlocks = new List<GameObject>();

    [SerializeField] private Tile[] m_spawnTiles;
    [SerializeField] private float m_spawnHeight = 10.0f;
    [SerializeField] private GameObject m_sandPrefab;

    // Called by button
    public void ResetSand()
    {
        // Destroy all sand
        for (int i = m_sandBlocks.Count - 1; i >= 0; i--)
        {
            Destroy(m_sandBlocks[i]);
        }

        // Spawn new sand
        for (int i = 0; i < m_spawnTiles.Length; i++)
        {
            GameObject newSand = Instantiate(m_sandPrefab, m_spawnTiles[i].transform.position, Quaternion.identity); // Spawn
            newSand.transform.parent = this.transform; // Set parent
            newSand.transform.position = newSand.transform.position + new Vector3(0.0f, m_spawnHeight, 0.0f); // Add height
            newSand.GetComponent<SandBlock>().Fall(); // Fall
        }
    }
}
