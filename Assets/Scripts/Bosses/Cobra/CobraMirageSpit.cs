using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraMirageSpit : MonoBehaviour
{
    [HideInInspector] public ECobraMirageType m_bulletType;
    public Transform m_bulletSpawn;
    public SkinnedMeshRenderer m_meshRenderer;
    public bool m_isReal = false;
    public bool m_headRaised = true;

    private Player m_playerRef;
    private GameObject m_redBulletPrefab;
    private GameObject m_blueBulletPrefab;
    private Material m_material;
    private CobraAnimations m_animations;
    private Collider m_hitBox;

    private void Awake()
    {
        m_redBulletPrefab = Resources.Load<GameObject>("Prefabs/Enemies/MirageBulletRed");
        m_blueBulletPrefab = Resources.Load<GameObject>("Prefabs/Enemies/MirageBulletBlue");
        m_playerRef = FindObjectOfType<Player>();
        m_meshRenderer.material = new Material(m_meshRenderer.material);
        m_material = m_meshRenderer.material;
        m_animations = GetComponent<CobraAnimations>();
        m_hitBox = GetComponent<Collider>();
    }

    public void FireProjectile()
    {
        MessageBus.TriggerEvent(EMessageType.cobraBarrageFire);

        if (m_isReal)
        {
            CobraMirageBarrage.s_shotsFired++;
        }

        GameObject bulletPrefab = (m_bulletType == ECobraMirageType.blue) ? m_blueBulletPrefab : m_redBulletPrefab;
        GameObject newBullet = Instantiate(bulletPrefab, m_bulletSpawn.position, transform.rotation, transform);
        EChunkEffect chunkEffect = (m_bulletType == ECobraMirageType.blue) ? EChunkEffect.water : EChunkEffect.fire;
        newBullet.GetComponent<MirageBullet>().Init(chunkEffect, m_playerRef.GetCurrentPower());

        Destroy(newBullet, CobraHealth.StateSettings.m_barrageProjectileLifetime);
        newBullet.GetComponent<Rigidbody>().velocity = transform.forward * CobraHealth.StateSettings.m_barrageProjectileSpeed;
    }

    public void Fade(bool _in)
    {
        float endValue = (_in) ? 0.0f : 1.0f;
        StopAllCoroutines();
        StartCoroutine(BossHelper.ChangeMaterialFloatPropertyOver(m_material, "_Cutoff", endValue, 2.5f));
    }

    public void LowerHead()
    {
        m_headRaised = false;
        StopAllCoroutines();
        // StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_Cutoff", 0.8f, 1.1f, 0.15f, true));
        // StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_FresnelStrength", 5.0f, 20.0f, 7.5f, true));
        m_animations.LowerHead();
        m_hitBox.enabled = true;
    }

    public void RaiseHead()
    {
        m_headRaised = true;
        StopAllCoroutines();
        // StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_Cutoff", 1.1f, 0.8f, -0.15f, false));
        // StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_FresnelStrength", 20.0f, 5.0f, -7.5f, false));
        
        if (!m_isReal)
        {
            Fade(_in: false);
        }

        m_animations.RaiseHead();
        m_hitBox.enabled = false;
    }

    public void ExitPot()
    {
        m_animations.ExitPot();
    }

    public void EnterPot()
    {
        m_animations.EnterPot();
    }

    public void CobraJump()
    {
        m_animations.CobraJump();
    }
}
