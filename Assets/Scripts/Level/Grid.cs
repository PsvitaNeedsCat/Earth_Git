using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Public variables


    // Private variables
    private Grid m_instance;
    private Tile[] m_tiles;
    [SerializeField] private GlobalPlayerSettings m_settings;

    private void Awake()
    {
        // Only one instance of this
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("A second instance of Grid.cs was instantiated");
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }

        m_tiles = FindObjectsOfType<Tile>();
        Debug.Assert(m_tiles.Length > 0, "No objects of type Tile.cs could be found");
    }

    // Finds the tile closest to a given location, EXCLUDING tiles of none
    public Tile FindClosestTile(Vector3 _queryPosition)
    {
        float closestDist = 1000.0f;
        Tile closestTile = null;

        for (int i = 0; i < m_tiles.Length; i++)
        {
            Tile tile = m_tiles[i];
            if (tile.GetTileType() == eType.none) { continue; }

            float dist = (tile.transform.position - _queryPosition).magnitude;
            if (dist < m_settings.m_minTileRange) { continue; }

            if (dist < closestDist)
            {
                closestTile = tile;
                closestDist = dist;
            }
        }

        return closestTile;
    }

    // Finds the tile closest to a given location, INCLDUING tiles of none
    public Tile FindClosestTileAny(Vector3 _queryPosition)
    {
        float closestDist = 1000.0f;
        Tile closestTile = null;

        for (int i = 0; i < m_tiles.Length; i++)
        {
            Tile tile = m_tiles[i];

            float dist = (tile.transform.position - _queryPosition).magnitude;

            if (dist < closestDist)
            {
                closestTile = tile;
                closestDist = dist;
            }
        }

        return closestTile;
    }
}
