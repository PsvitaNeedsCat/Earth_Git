using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadSpit : ToadBehaviour
{
    // Height above tile to drop projectile from
    public float m_spawnHeight = 100.0f;
    public Transform m_projectileSpawnSocket;
    public float m_projLaunchForce = 2.0f;

    public static Dictionary<int, Tile> m_levelTiles = new Dictionary<int, Tile>();

    GameObject m_projectilePrefab;
    ToadBoss m_toadBoss;

    private void Awake()
    {
        m_projectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Toad/ToadSpitProjectile");
        m_toadBoss = GetComponent<ToadBoss>();
    }

    private void Start()
    {
        // Store all level tiles in a dictionary
        m_levelTiles.Clear();
        List<Tile> tiles = Grid.GetTiles();
        Debug.Log("Spit attack found " + tiles.Count + " tiles");
        for (int i = 0; i < tiles.Count; i++)
        {
            m_levelTiles.Add(tiles[i].GetInstanceID(), tiles[i]);
        }
    }

    public override void StartBehaviour()
    {
        Start();
        base.StartBehaviour();
        m_toadAnimator.SetTrigger("Spit");
    }

    public override void Reset()
    {
        base.Reset();
        ToadBoss.m_eaten = eChunkType.none;
    }

    public void AESpitProjectile() => StartCoroutine(SpitProjectile());

    IEnumerator SpitProjectile()
    {
        // Notify message bus
        MessageBus.TriggerEvent(EMessageType.toadSpit);

        // Create projectile
        GameObject newProjectile = Instantiate(m_projectilePrefab, m_projectileSpawnSocket.position, Quaternion.identity, null);
        ToadSpitProjectile proj = newProjectile.GetComponent<ToadSpitProjectile>();

        // Find a tile for it to aim for
        Tile aimTile = GetRandomFreeTile();
        proj.m_aimedTile = aimTile;
        ProjectileCreated(aimTile);

        // Launch projectile upwards
        proj.m_rigidbody.AddForce(Vector3.up * m_projLaunchForce, ForceMode.Impulse);

        // If boss has eaten rock, notify projectile
        if (ToadBoss.m_eaten == eChunkType.rock) proj.m_shouldSplit = true;

        // Wait until projectile is off-screen
        yield return new WaitForSeconds(0.5f);

        // When projectile is off-screen, move it above the tile it will land on
        proj.m_rigidbody.velocity = Vector3.zero;
        proj.transform.position = proj.m_aimedTile.transform.position + Vector3.up * m_spawnHeight;
    }

    // Removes aimed tile from the dictionary, so another shot can't be fired at the same tile
    public static void ProjectileCreated(Tile _aimedTile)
    {
        m_levelTiles.Remove(_aimedTile.GetInstanceID());
    }
    
    // Re-adds tile to the dictionary, because the projectile aiming at it has been destroyed
    public static void ProjectileDestroyed(Tile _aimedTile)
    {
        m_levelTiles.Add(_aimedTile.GetInstanceID(), _aimedTile);
    }

    private Tile GetRandomFreeTile()
    {
        // Get list of keys from dict
        List<int> keyList = new List<int>(m_levelTiles.Keys);
        
        // Get a key at random, and return the tile at that key
        int randomIndex = Random.Range(0, keyList.Count);
        int randomKey = keyList[randomIndex];
        return m_levelTiles[randomKey];
    }
}
