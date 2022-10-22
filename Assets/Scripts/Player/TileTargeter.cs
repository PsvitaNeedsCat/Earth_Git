using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileTargeter : MonoBehaviour
{
    // Public variables
    public LayerMask m_tileLayerMask;
    public Vector3 m_boxHalfExtents = new Vector3(0.5f, 0.6f, 0.5f);
    public float m_minTileRange;
    public GameObject m_targeterRockParticles;

    // Private variables
    private TileTargeter m_instance;
    private Tile m_closestTile;
    private GlobalPlayerSettings m_settings;
    [SerializeField] GameObject m_indicator;
    private Vector2 m_direction = Vector2.zero;
    private Renderer m_targeterRockRenderer;

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
        m_targeterRockRenderer = m_targeterRockParticles.GetComponent<Renderer>();
    }

    // Update is only called when tile targeter is active
    private void Update()
    {
        // Don't target anything if the player is not moving the LAnalogStick
        /*if (m_direction == Vector2.zero)
        {
            Activate(false);
            m_closestTile = null; 
            return; 
        }*/

        if (m_closestTile != null)
        {
            m_closestTile.SetHighlighted(false);
        }

        // m_closestTile = Grid.FindClosestTile(transform.position, transform.parent.transform.position);
        m_closestTile = FindClosestTile();

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
            m_targeterRockRenderer.material = m_closestTile.GetMaterial();
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
        if (_useRelativeCamera) 
        {
            direction = Camera.main.RelativeDirection2(direction); 
        }

        if (!m_settings) 
        {
            m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings"); 
        }

        direction = direction.normalized * m_settings.m_TargeterMoveDist;

        // Move tile targeter accordingly
        transform.position = _playerPos + direction;
    }

    public void ResetDirection()
    {
        m_direction = Vector2.zero;
    }

    private Tile FindClosestTile()
    {
        Transform playerTransform = transform.parent;
        Vector3 playerPosition = playerTransform.position;

        Collider[] hits = Physics.OverlapBox(transform.position, m_boxHalfExtents, Quaternion.identity, m_tileLayerMask);
        List<Tile> tiles = new List<Tile>();

        // Find all tiles hit
        for (int i = 0; i < hits.Length; i++)
        {
            Tile hitTile = hits[i].transform.parent.GetComponent<Tile>();

            if (hitTile != null)
            {
                tiles.Add(hitTile);
            }
        }

        // Order tiles by distance to tile targeter
        tiles = tiles.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();

        for (int i = 0; i < tiles.Count; i++)
        {
            Tile checkTile = tiles[i];
            EChunkType chunkType = checkTile.GetTileType();

            if (chunkType == EChunkType.none || chunkType == EChunkType.lava)
            {
                continue;
            }

            float distToPlayer = Vector3.Distance(checkTile.transform.position, playerPosition);

            if (!checkTile.IsOccupied() && distToPlayer > m_minTileRange)
            {
                return checkTile;
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.cyan;

        Transform playerTransform = transform.parent;

        Gizmos.DrawWireCube(transform.position, m_boxHalfExtents * 2.0f);
    }
}
