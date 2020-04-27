using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Public variables


    // Private variables
    private Grid m_instance;
    static List<Tile> m_tiles = new List<Tile>();
    static GlobalPlayerSettings m_playerSettings;

    public static void AddTile(Tile _newTile) => m_tiles.Add(_newTile);
    public static void RemoveTile(Tile _removeTile) => m_tiles.Remove(_removeTile);

    private void Awake()
    {
        // Only one instance of this
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }

        m_playerSettings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
    }

    // Finds the closest tile to a query position, either including or excluding tiles of type 'none', that can't be raised
    public static Tile FindClosestTile(Vector3 _queryPosition, bool _includeUnraisable)
    {
        float closestDist = float.MaxValue;
        Tile closestTile = null;

        // Check every tile
        foreach (Tile tile in m_tiles)
        {
            // If not including tiles of type 'none', and this is one, skip it
            if (!_includeUnraisable && tile.GetTileType() == eChunkType.none) { continue; }

            float dist = (tile.transform.position - _queryPosition).magnitude;
            if (dist < m_playerSettings.m_minTileRange) { continue; }

            // If this tile is closer than current closest, update current closest
            if (dist < closestDist)
            {
                closestTile = tile;
                closestDist = dist;
            }
        }

        return closestTile;
    }
}
