using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using UnityEditor;

public class CobraPot : MonoBehaviour
{
    public Transform m_projectileParent;
    public float m_projectileSpawnDistance;
    public float m_projectileSpawnHeight;
    public float m_lobProjectileSpawnHeight;
    public float m_lobVelocity;
    public Vector3 m_lobDir;
    public LayerMask m_tileLayers;
    public GameObject m_mesh;
    public Transform m_moveTransform;

    public GameObject m_potLandIndicator;
    public List<GameObject> m_potProjectileLandIndicators;

    public int m_potIndex = -1;
    public int m_endIndex = -1;
    public Quaternion m_endRotation;

    private GameObject m_projectilePrefab;
    private GameObject m_lobProjectilePrefab;
    private Collider m_collider;
    private CobraAnimations m_animations;
    
    private void Awake()
    {
        m_projectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/CobraPotProjectile");
        m_lobProjectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/CobraPotLobProjectile");
        m_collider = GetComponentInChildren<Collider>();
        m_animations = GetComponent<CobraAnimations>();

        m_potLandIndicator.SetActive(false);
        for (int i = 0; i < m_potProjectileLandIndicators.Count; i++)
        {
            m_potProjectileLandIndicators[i].SetActive(false);
        }
    }

    public Transform GetMoveTransform()
    {
        return m_moveTransform;
    }

    public void SetCollider(bool _active)
    {
        m_collider.enabled = _active;
    }

    public void FireProjectile()
    {
        MessageBus.TriggerEvent(EMessageType.cobraPotFire);

        // Create a projectile
        Vector3 spawnPosition = m_moveTransform.position + m_moveTransform.forward * m_projectileSpawnDistance + Vector3.up * m_projectileSpawnHeight;
        GameObject projectile = Instantiate(m_projectilePrefab, spawnPosition, m_moveTransform.rotation, m_projectileParent);
        Destroy(projectile, CobraHealth.StateSettings.m_potProjectileLifetime);
        projectile.GetComponent<Rigidbody>().velocity = m_moveTransform.forward * CobraHealth.StateSettings.m_potProjectileSpeed;

        // Pot firing animation
        m_animations.PotFire();
    }

    public void FireLobProjectiles()
    {
        FireAtSurroundingTiles();
    }

    private void LobProjectile(Vector3 _dir)
    {
        Vector3 spawnPosition = m_moveTransform.position + m_moveTransform.up * m_lobProjectileSpawnHeight;
        GameObject lobProjectile = Instantiate(m_lobProjectilePrefab, spawnPosition, m_moveTransform.rotation, m_projectileParent);
        // Destroy(lobProjectile, CobraHealth.StateSettings.m_potProjectileLifetime);

        lobProjectile.GetComponent<Rigidbody>().velocity = _dir * m_lobVelocity;
    }

    public void EnablePotProjectileIndicator(GameObject _projectileIndicator, Vector3 _destination)
    {
        _projectileIndicator.transform.position = _destination;
        
        _projectileIndicator.SetActive(true);
    }

    public IEnumerator DisablePotProjectileIndicatorsAfter(float _seconds)
    {
        yield return new WaitForSeconds(_seconds);

        for (int i = 0; i < m_potProjectileLandIndicators.Count; i++)
        {
            m_potProjectileLandIndicators[i].SetActive(false);
        }
    }

    public void EnablePotIndicator(Vector3 _destination)
    {
        m_potLandIndicator.transform.position = _destination;
        m_potLandIndicator.SetActive(true);
    }

    public void DisablePotIndicator()
    {
        m_potLandIndicator.SetActive(false);
    }

    private void FireAtSurroundingTiles()
    {
        Vector3 lobDir = m_moveTransform.rotation * m_lobDir.normalized;

        // Forward
        if (CheckForTile(m_moveTransform.forward))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, lobDir.z));
            EnablePotProjectileIndicator(m_potProjectileLandIndicators[0], m_moveTransform.position + m_moveTransform.forward);
        }

        lobDir = Quaternion.Euler(0.0f, 90.0f, 0.0f) * lobDir;

        // Right
        if (CheckForTile(m_moveTransform.right))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, lobDir.z));
            EnablePotProjectileIndicator(m_potProjectileLandIndicators[1], m_moveTransform.position + m_moveTransform.right);
        }

        lobDir = Quaternion.Euler(0.0f, 90.0f, 0.0f) * lobDir;

        // Back
        if (CheckForTile(-m_moveTransform.forward))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, lobDir.z));
            EnablePotProjectileIndicator(m_potProjectileLandIndicators[2], m_moveTransform.position - m_moveTransform.forward);
        }

        lobDir = Quaternion.Euler(0.0f, 90.0f, 0.0f) * lobDir;

        // Left
        if (CheckForTile(-m_moveTransform.right))
        {
            LobProjectile(new Vector3(lobDir.x, lobDir.y, lobDir.z));
            EnablePotProjectileIndicator(m_potProjectileLandIndicators[3], m_moveTransform.position - m_moveTransform.right);
        }

        StartCoroutine(DisablePotProjectileIndicatorsAfter(1.0f));
    }

    private bool CheckForTile(Vector3 _dir)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(m_moveTransform.position + _dir, -Vector3.up);

        if (Physics.Raycast(ray, out hitInfo, 5.0f, m_tileLayers))
        {
            return (hitInfo.collider.GetComponentInParent<Tile>());
        }

        return false;
    }

    public void JumpOut(float _overSeconds)
    {
        Vector3 finalPosition = CobraShuffle.s_potStartingPositions[m_endIndex];
        m_moveTransform.DOMove(finalPosition, _overSeconds);
        m_moveTransform.DORotateQuaternion(m_endRotation, _overSeconds);
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
        Gizmos.DrawLine(m_moveTransform.position, m_moveTransform.position + m_lobDir.normalized);

#if UNITY_EDITOR
        Handles.Label(m_moveTransform.position + Vector3.up * 0.5f, "CIndex: " + m_potIndex.ToString());
        Handles.Label(m_moveTransform.position + Vector3.up * 0.25f, "EIndex: " + m_endIndex.ToString());
#endif
    }
}
