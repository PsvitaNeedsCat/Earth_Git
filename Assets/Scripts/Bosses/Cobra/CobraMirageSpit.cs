using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraMirageSpit : MonoBehaviour
{
    [HideInInspector] public ECobraMirageType m_bulletType;
    public Transform m_bulletSpawn;

    private Player m_playerRef;
    private GameObject m_bulletPrefab;

    private void Awake()
    {
        m_bulletPrefab = Resources.Load<GameObject>("Prefabs/Enemies/MirageBullet");
        m_playerRef = FindObjectOfType<Player>();
    }

    public void FireProjectile()
    {
        GameObject newBullet = Instantiate(m_bulletPrefab, m_bulletSpawn.position, transform.rotation, transform);
        eChunkEffect chunkEffect = (m_bulletType == ECobraMirageType.blue) ? eChunkEffect.water : eChunkEffect.fire;
        newBullet.GetComponent<MirageBullet>().Init(chunkEffect, m_playerRef.GetCurrentPower());

        Destroy(newBullet, CobraHealth.StateSettings.m_barrageProjectileLifetime);
        newBullet.GetComponent<Rigidbody>().velocity = transform.forward * CobraHealth.StateSettings.m_barrageProjectileSpeed;
    }
}
