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
            Activate(false);
            m_closestTile = null; 
            return; 
        }

        if (m_closestTile != null)
        {
            m_closestTile.SetHighlighted(false);
        }

        m_closestTile = Grid.FindClosestTile(transform.position, transform.parent.transform.position);

        // Debug.LogError(m_closestTile);

        if (!m_closestTile || m_closestTile.IsOccupied())
        {
            Activate(false);
            return; 
        }

        Vector3 diff = m_closestTile.transform.position - transform.position;
        diff.y = 0.0f;

        // Check if closest is within maximum range && is not occupied
        if (diff.magnitude < m_settings.m_maxTileRange)
        {
            Activate(true);
            m_indicator.transform.position = m_closestTile.transform.position;
            m_indicator.transform.rotation = m_closestTile.transform.rotation;
            // Debug.LogError("Targeting tile with transform: " + m_closestTile.transform.rotation);
        }
        else
        {
            Activate(false);
            m_closestTile = null;
        }
    }

    private void OnEnable()
    {
        //Activate(true);
    }

    private void OnDisable()
    {
        Activate(false);
    }

    private void Activate(bool _active)
    {
        m_indicator.SetActive(_active);

        if (m_closestTile != null)
        {
            m_closestTile.SetHighlighted(_active);
        }
    }

    // Returns the closest tile
    public Tile GetClosest()
    {
        return m_closestTile;
    }

    // Changes the direction from the player that the tile should try and target
    public void SetTargetDirection(Vector2 _dir, Vector3 _playerPos, bool _useRelativeCamera = true)
    {
        m_direction = _dir;

        UpdateDirection(_playerPos, _useRelativeCamera);
    }

    public void UpdateDirection(Vector3 _playerPos, bool _useRelativeCamera = true)
    {
        Vector3 direction = m_direction;

        // Move direction by camera
        if (_useRelativeCamera) { direction = Camera.main.RelativeDirection2(direction); }

        if (!m_settings) { m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings"); }

        direction = direction.normalized * m_settings.m_TargeterMoveDist;

        // Move tile targeter accordingly
        transform.position = _playerPos + direction;
    }

    public void ResetDirection()
    {
        m_direction = Vector2.zero;
    }
}
