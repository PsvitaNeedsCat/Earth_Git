using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeTailFirer : MonoBehaviour
{
    public GameObject m_projectilePrefab;
    public float m_spawnDistance;
    public float m_spawnHeight;

    public void FireAll()
    {
        Fire(transform.localRotation * Vector3.right);
        Fire(transform.localRotation * Vector3.left);
        Fire(transform.localRotation * Vector3.forward);
        Fire(transform.localRotation * Vector3.back);
    }

    private void Fire(Vector3 _dir)
    {
        GameObject newProjectile = Instantiate(m_projectilePrefab, transform.position + m_spawnDistance * _dir + Vector3.up * m_spawnHeight, Quaternion.identity, null);
        newProjectile.GetComponent<Rigidbody>().velocity = _dir * CentipedeBoss.m_settings.m_projectileSpeed;
    }
}
