using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraMirageSpit : MonoBehaviour
{
    [HideInInspector] public ECobraMirageType m_bulletType;
    public Transform m_bulletSpawn;
    public SkinnedMeshRenderer m_meshRenderer;

    private Player m_playerRef;
    private GameObject m_bulletPrefab;
    private Material m_material;
    private CobraAnimations m_animations;

    private void Awake()
    {
        m_bulletPrefab = Resources.Load<GameObject>("Prefabs/Enemies/MirageBullet");
        m_playerRef = FindObjectOfType<Player>();
        m_meshRenderer.material = new Material(m_meshRenderer.material);
        m_material = m_meshRenderer.material;
        m_animations = GetComponent<CobraAnimations>();
    }

    public void FireProjectile()
    {
        MessageBus.TriggerEvent(EMessageType.cobraBarrageFire);

        GameObject newBullet = Instantiate(m_bulletPrefab, m_bulletSpawn.position, transform.rotation, transform);
        EChunkEffect chunkEffect = (m_bulletType == ECobraMirageType.blue) ? EChunkEffect.water : EChunkEffect.fire;
        newBullet.GetComponent<MirageBullet>().Init(chunkEffect, m_playerRef.GetCurrentPower());

        Destroy(newBullet, CobraHealth.StateSettings.m_barrageProjectileLifetime);
        newBullet.GetComponent<Rigidbody>().velocity = transform.forward * CobraHealth.StateSettings.m_barrageProjectileSpeed;
    }

    public void LowerHead()
    {
        StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_Cutoff", 0.8f, 1.1f, 0.15f, true));
        StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_FresnelStrength", 5.0f, 20.0f, 7.5f, true));
        m_animations.LowerHead();
    }

    public void RaiseHead()
    {
        StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_Cutoff", 1.1f, 0.8f, -0.15f, false));
        StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_FresnelStrength", 20.0f, 5.0f, -7.5f, false));
        m_animations.RaiseHead();
    }

    public void ExitPot()
    {
        m_animations.ExitPot();
    }

    public void EnterPot()
    {
        m_animations.EnterPot();
    }
}
