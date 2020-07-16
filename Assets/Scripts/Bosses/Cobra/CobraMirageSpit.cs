using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageSpit : MonoBehaviour
{
    public ECobraMirageType m_bulletType;
    public Transform m_bulletSpawn;

    private GameObject m_bulletPrefab;

    private void Awake()
    {
        m_bulletPrefab = (m_bulletType == ECobraMirageType.blue) ? Resources.Load<GameObject>("Prefabs/Bosses/Cobra/MirageBulletBlue") : Resources.Load<GameObject>("Prefabs/Bosses/Cobra/MirageBulletRed");
    }

    public void FireProjectile()
    {
        GameObject newBullet = Instantiate(m_bulletPrefab, m_bulletSpawn.position, transform.rotation, transform);
        Destroy(newBullet, CobraHealth.StateSettings.m_barrageProjectileLifetime);
        newBullet.GetComponent<Rigidbody>().velocity = transform.forward * CobraHealth.StateSettings.m_barrageProjectileSpeed;
    }
}
