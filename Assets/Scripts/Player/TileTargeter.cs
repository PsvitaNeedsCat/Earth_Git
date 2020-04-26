using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTargeter : MonoBehaviour
{
    // Public variables


    // Private variables
    private TileTargeter m_instance;
    private Grid m_grid;
    private Tile m_closestTile;
    [SerializeField] private GlobalPlayerSettings m_settings;
    [SerializeField] GameObject m_indicator;

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
    }

    private void OnEnable()
    {
        // Get a reference to the grid
        if (!m_grid)
        {
            m_grid = FindObjectOfType<Grid>();
            Debug.Assert(m_grid, "No object of type Grid.cs could be found");
        }
    }

    // Update is only called when tile targeter is active
    private void Update()
    {
        m_closestTile = m_grid.FindClosestTile(transform.position);

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
}
