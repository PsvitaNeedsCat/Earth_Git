using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTargeter : MonoBehaviour
{
    // Public variables


    // Private variables
    private TileTargeter m_instance;
    private Tile m_closestTile;
    private GlobalPlayerSettings m_settings;
    [SerializeField] GameObject m_indicator;
    private Vector2 m_direction = Vector2.zero;

    private void Awake()
    {
        // Only one instance of this
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("A second instance of TileTargeter.cs was instantiated");
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }

        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
    }

    // Update is only called when tile targeter is active
    private void Update()
    {
        // Don't target anything if the player is not moving the LAnalogStick
        if (m_direction == Vector2.zero)
        {
            m_indicator.SetActive(false);
            m_closestTile = null; 
            return; 
        }

        m_closestTile = Grid.FindClosestTile(transform.position, transform.parent.transform.position);

        if (!m_closestTile) { return; }

        Vector3 diff = m_closestTile.transform.position - transform.position;
        diff.y = 0.0f;

        // Check if closest is within maximum range && is not occupied
        if (diff.magnitude < m_settings.m_maxTileRange && !m_closestTile.IsOccupied())
        {
            m_indicator.SetActive(true);
            m_indicator.transform.position = m_closestTile.transform.position;
            m_indicator.transform.rotation = m_closestTile.transform.rotation;
        }
        else
        {
            m_indicator.SetActive(false);
            m_closestTile = null;
        }
    }

    // Returns the closest tile
    public Tile GetClosest()
    {
        return m_closestTile;
    }

    // Changes the direction from the player that the tile should try and target
    public void SetTargetDirection(Vector2 _dir, Vector3 _playerPos)
    {
        m_direction = _dir;

        // Move direction by camera
        Vector3 direction = Camera.main.RelativeDirection2(_dir);

        direction = direction.normalized * m_settings.m_TargeterMoveDist;

        // Move tile targeter accordingly
        transform.position = _playerPos + direction;
    }

    public void ResetDirection()
    {
        m_direction = Vector2.zero;
    }
}
