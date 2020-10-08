using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraMirageSpit : MonoBehaviour
{
    public Transform m_bulletSpawn;
    public SkinnedMeshRenderer m_meshRenderer;
    public bool m_isReal = false;
    public bool m_headRaised = true;

    [SerializeField] private ParticleSystem[] m_shieldParticles;

    private GameObject m_bulletPrefab;
    private Material m_normalMaterial;
    private CobraAnimations m_animations;
    private Collider m_hitBox;

    private void Awake()
    {
        m_bulletPrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/CobraPotProjectile");

        m_meshRenderer.material = new Material(m_meshRenderer.material);
        m_normalMaterial = m_meshRenderer.material;

        m_animations = GetComponent<CobraAnimations>();
        m_hitBox = GetComponent<Collider>();

        if (!m_isReal)
        {
            Fade(false);
            SetShieldMaterial(false);
        }
    }

    public void FireProjectile()
    {
        if (m_isReal)
        {
            MessageBus.TriggerEvent(EMessageType.cobraBarrageFire);
            CobraMirageBarrage.s_shotsFired++;
        }

        GameObject newBullet = Instantiate(m_bulletPrefab, m_bulletSpawn.position, transform.rotation, transform);

        Destroy(newBullet, CobraHealth.StateSettings.m_barrageProjectileLifetime);
        newBullet.GetComponent<Rigidbody>().velocity = transform.forward * CobraHealth.StateSettings.m_barrageProjectileSpeed;
    }

    public void LowerHead()
    {
        m_headRaised = false;
        StopAllCoroutines();

        SetShieldMaterial(false);

        m_animations.LowerHead();
        m_hitBox.enabled = true;
    }

    public void RaiseHead()
    {
        m_headRaised = true;
        StopAllCoroutines();

        SetShieldMaterial(true);

        if (!m_isReal)
        {
            Fade(_in: false);
        }

        m_animations.RaiseHead();
        m_hitBox.enabled = false;
    }

    private void SetShieldMaterial(bool _on)
    {
        if (m_isReal)
        {
            Debug.Log("Boss set shield material " + _on);
        }

        float cutoffEndValue = (_on) ? 0.5f : 1.1f;
        float fresnelEndValue = (_on) ? 5.0f : 20.0f;
        float transitionDuration = 2.0f;

        StartCoroutine(BossHelper.ChangeMaterialFloatPropertyOver(m_normalMaterial, "_ShieldCutoff", cutoffEndValue, transitionDuration));
        //StartCoroutine(BossHelper.ChangeMaterialFloatPropertyOver(m_normalMaterial, "_FresnelStrength", fresnelEndValue, transitionDuration));

        if (_on)
        {
            m_shieldParticles[1].Play();
        }
        else
        {
            m_shieldParticles[0].Play();
        }
    }

    public void Fade(bool _in)
    {
        float endValue = (_in) ? 0.0f : 1.0f;
        StopAllCoroutines();
        StartCoroutine(BossHelper.ChangeMaterialFloatPropertyOver(m_normalMaterial, "_Cutoff", endValue, 2.0f));
    }

    public void ExitPotFade()
    {
        m_animations.ExitPot();

        StopAllCoroutines();

        Fade(true);
        SetShieldMaterial(true);
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
