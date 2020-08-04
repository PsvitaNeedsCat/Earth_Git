using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using UnityEditor;

public class CobraPot : MonoBehaviour
{
    public Transform m_projectileParent;
    public float m_projectileSpawnDistance;
    public float m_lobProjectileSpawnHeight;
    public float m_lobVelocity;
    public Vector3 m_lobDir;
    public LayerMask m_tileLayers;
    public GameObject m_mesh;

    public int m_potIndex = -1;
    public int m_endIndex = -1;
    public Quaternion m_endRotation;

    private GameObject m_projectilePrefab;
    private GameObject m_lobProjectilePrefab;
    
    private void Awake()
    {
        m_projectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/CobraPotProjectile");
        m_lobProjectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/CobraPotLobProjectile");
    }

    public void FireProjectile()
    {
        MessageBus.TriggerEvent(EMessageType.cobraPotFire);

        // Create a projectile
        Vector3 spawnPosition = transform.position + transform.forward * m_projectileSpawnDistance;
        GameObject projectile = Instantiate(m_projectilePrefab, spawnPosition, transform.rotation, m_projectileParent);
        Destroy(projectile, CobraHealth.StateSettings.m_potProjectileLifetime);
        projectile.GetComponent<Rigidbody>().velocity = transform.forward * CobraHealth.StateSettings.m_potProjectileSpeed;

        // Temp minor visual feedback for firing
        transform.DOPunchScale(Vector3.one * 0.2f, 0.15f).SetEase(Ease.InOutElastic);
        transform.DOPunchPosition(Vector3.up * 0.2f, 0.2f).SetEase(Ease.InCubic);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            FireAtSurroundingTiles();
        }
    }

    public void FireLobProjectiles()
    {
        FireAtSurroundingTiles();
    }

    private void LobProjectile(Vector3 _dir)
    {
        Vector3 spawnPosition = transform.position + transform.up * m_lobProjectileSpawnHeight;
        GameObject lobProjectile = Instantiate(m_lobProjectilePrefab, spawnPosition, transform.rotation, m_projectileParent);
        // Destroy(lobProjectile, CobraHealth.StateSettings.m_potProjectileLifetime);

        lobProjectile.GetComponent<Rigidbody>().velocity = _dir * m_lobVelocity;
    }

    private void FireAtSurroundingTiles()
    {
        Vector3 lobDir = transform.rotation * m_lobDir.normalized;
        

        // Forward
        if (CheckForTile(transform.forward))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, lobDir.z));
        }

        lobDir = Quaternion.Euler(0.0f, 90.0f, 0.0f) * lobDir;

        // Right
        if (CheckForTile(transform.right))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, lobDir.z));
        }

        lobDir = Quaternion.Euler(0.0f, 90.0f, 0.0f) * lobDir;

        // Back
        if (CheckForTile(-transform.forward))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, lobDir.z));
        }

        lobDir = Quaternion.Euler(0.0f, 90.0f, 0.0f) * lobDir;

        // Left
        if (CheckForTile(-transform.right))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, lobDir.z));
        }
    }

    private bool CheckForTile(Vector3 _dir)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position + _dir, -Vector3.up);

        if (Physics.Raycast(ray, out hitInfo, 5.0f, m_tileLayers))
        {
            return (hitInfo.collider.GetComponentInParent<Tile>());
        }

        return false;
    }

    public void JumpOut(float _overSeconds)
    {
        Vector3 finalPosition = CobraShuffle.s_potStartingPositions[m_endIndex];
        transform.DOMove(finalPosition, _overSeconds);
        transform.DORotateQuaternion(m_endRotation, _overSeconds);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            player.GetComponent<HealthComponent>().Health -= 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + m_lobDir.normalized);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.forward, transform.position + transform.forward + -Vector3.up * 5.0f);
        Gizmos.DrawLine(transform.position + transform.right, transform.position + transform.right + -Vector3.up * 5.0f);
        Gizmos.DrawLine(transform.position - transform.forward, transform.position - transform.forward + -Vector3.up * 5.0f);
        Gizmos.DrawLine(transform.position - transform.right, transform.position - transform.right + -Vector3.up * 5.0f);


#if UNITY_EDITOR
        Handles.Label(transform.position + Vector3.up * 0.5f, "CIndex: " + m_potIndex.ToString());
        Handles.Label(transform.position + Vector3.up * 0.25f, "EIndex: " + m_endIndex.ToString());
#endif
    }
}
