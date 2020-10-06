using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Public variables
    public static readonly float s_tileSize = 1.0f;

    // Private variables
    private Grid m_instance;
    static List<Tile> s_tiles = new List<Tile>();
    static GlobalPlayerSettings s_playerSettings;

    public static void AddTile(Tile _newTile)
    {
        if (!s_tiles.Contains(_newTile)) s_tiles.Add(_newTile);

        // Debug.Log("Tile added. Count: " + m_tiles.Count);
    }

    public static void RemoveTile(Tile _removeTile)
    {
        s_tiles.Remove(_removeTile);
        // Debug.Log("Tile removed. Count: " + m_tiles.Count);
    }

    public static List<Tile> GetTiles() { return s_tiles; }

    private void Awake()
    {
        // Only one instance of this
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }

        s_playerSettings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
    }

    // Finds the closest tile to a query position - excluding tiles that are too close, or tiles that cannot be raised
    public static Tile FindClosestTile(Vector3 _queryPosition, Vector3 _playerPos)
    {
        float closestDist = float.MaxValue;
        Tile closestTile = null;

        // Check every tile
        foreach (Tile tile in s_tiles)
        {
            // If not including tiles of type 'none', and this is one, skip it
            if (tile.GetTileType() == EChunkType.none || tile.GetTileType() == EChunkType.lava || tile.m_ignore) { continue; }

            Vector3 toPlayer = tile.transform.position - _playerPos;
            toPlayer.y = 0.0f;

            if (toPlayer.magnitude < s_playerSettings.m_minTileRange) { continue; }

            float dist = (tile.transform.position - _queryPosition).magnitude;

            // If this tile is closer than current closest, update current closest
            if (dist < closestDist)
            {
                closestTile = tile;
                closestDist = dist;
            }
        }

        return closestTile;
    }

    // Finds the closest tile with no restrictions
    public static Tile FindClosestTileAny(Vector3 _queryPosition)
    {
        float closestDist = float.MaxValue;
        Tile closestTile = null;

        foreach (Tile tile in s_tiles)
        {
            float dist = (tile.transform.position - _queryPosition).magnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closestTile = tile;
            }
        }

        return closestTile;
    }
}
