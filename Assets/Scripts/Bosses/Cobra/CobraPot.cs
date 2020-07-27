using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraPot : MonoBehaviour
{
    public Transform m_projectileParent;
    public float m_projectileSpawnDistance;
    public float m_lobProjectileSpawnHeight;
    public float m_lobVelocity;
    public Vector3 m_lobDir;
    public LayerMask m_tileLayers;
    public GameObject m_mesh;

    private GameObject m_projectilePrefab;
    private GameObject m_lobProjectilePrefab;

    private void Awake()
    {
        m_projectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/CobraPotProjectile");
        m_lobProjectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/CobraPotLobProjectile");
    }

    public void FireProjectile()
    {
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

    private void LobProjectile(Vector3 _dir)
    {
        Vector3 spawnPosition = transform.position + transform.up * m_lobProjectileSpawnHeight;
        GameObject lobProjectile = Instantiate(m_lobProjectilePrefab, spawnPosition, transform.rotation, m_projectileParent);
        // Destroy(lobProjectile, CobraHealth.StateSettings.m_potProjectileLifetime);

        lobProjectile.GetComponent<Rigidbody>().velocity = _dir * m_lobVelocity;
    }

    private void FireAtSurroundingTiles()
    {
        Vector3 lobDir = m_lobDir.normalized;

        // Forward
        if (CheckForTile(transform.forward))
        {
            LobProjectile(lobDir);
        }

        // Right
        if (CheckForTile(transform.right))
        {
            LobProjectile(new Vector3(lobDir.z, lobDir.y, lobDir.x));
        }

        // Back
        if (CheckForTile(-transform.forward))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, -lobDir.z));
        }

        // Left
        if (CheckForTile(-transform.right))
        {
            LobProjectile(new Vector3(-lobDir.z, lobDir.y, lobDir.x));
        }
    }

    private bool CheckForTile(Vector3 _dir)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position + _dir, -Vector3.up);

        if (Physics.Raycast(ray, out hitInfo, 5.0f, m_tileLayers))
        {
            Debug.Log("Hit " + hitInfo.collider.name);
            return (hitInfo.collider.GetComponentInParent<Tile>());
        }

        return false;
    }

    //private Vector3 DirectionFromAngle(float _angle)
    //{
    //    Vector3 final = Quaternion.Euler(_angle, 0.0f, 0.0f) * Vector3.forward;
    //    return final.normalized;
    //}

    //// vv Whether to use plus or minus in the ± part of the equation
    //private float GetAngleForProjectile(float _vInitial, float _deltaX, float _deltaY, float _gravity, bool _plus)
    //{
    //    float toSqrt = (_vInitial * _vInitial * _vInitial * _vInitial) - _gravity * (_gravity * _deltaX * _deltaX + 2 * _deltaY * _vInitial * _vInitial);
    //    float afterSqrt = Mathf.Sqrt(toSqrt);

    //    // Choose + or - based on input
    //    float signedAfterSqrt = (_plus) ? afterSqrt : -afterSqrt;

    //    float finalValue = Mathf.Rad2Deg * Mathf.Tan((_vInitial * _vInitial + signedAfterSqrt) / _gravity * _deltaX);

    //    return finalValue;
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + m_lobDir.normalized);
    }
}
