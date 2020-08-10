using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToadSwell : ToadBehaviour
{
    public SkinnedMeshRenderer m_toadRenderer;
    // public Material m_swollenMaterial;
    public Transform m_meshTransform;
    public Texture m_swollenTexture;

    private ToadBossSettings m_toadSettings;
    private float m_swelledTimer = 0.0f;
    private float m_startingScale;
    private Material m_material;
    private Texture m_normalTexture;
    private HealthComponent m_toadHealth;

    private void Awake()
    {
        m_toadSettings = Resources.Load<ToadBossSettings>("ScriptableObjects/ToadBossSettings");
        m_startingScale = m_meshTransform.localScale.x;
        // m_normalMaterial = m_toadRenderer.material;
        m_toadRenderer.material = new Material(m_toadRenderer.material);
        m_material = m_toadRenderer.material;
        m_normalTexture = m_material.mainTexture;
        m_toadHealth = GetComponent<HealthComponent>();
        m_toadHealth.IsInvincible = true;
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        StartCoroutine(Swell());
    }

    public override void Reset()
    {
        base.Reset();
        m_swelledTimer = 0.0f;
    }

    private IEnumerator Swell()
    {
        SwellUp();

        // Wait for toad to take damage or timer to run out before swelling down
        while (!ToadBoss.s_tookDamage && m_swelledTimer < m_toadSettings.m_staySwelledUpFor)
        {
            m_swelledTimer += Time.deltaTime;
            yield return null;
        }

        SwellDown();
    }

    void SwellUp()
    {
        m_toadAnimator.SetTrigger("SwellUp");
        m_toadHealth.IsInvincible = false;
        m_meshTransform.DOKill();
        m_meshTransform.DOScale(m_startingScale * 1.1f, m_toadSettings.m_swellUpOver).SetEase(Ease.OutElastic);
        // m_toadRenderer.material = m_swollenMaterial;
        m_material.SetTexture("_MainTex", m_swollenTexture);

        StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_Cutoff", 0.8f, 1.1f, 0.15f, true));
        StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_FresnelStrength", 5.0f, 20.0f, 7.5f, true));

        MessageBus.TriggerEvent(EMessageType.vulnerableStart);
    }

    void SwellDown()
    {
        m_toadAnimator.SetTrigger("SwellDown");

        ToadBoss.s_tookDamage = false;
        m_toadHealth.IsInvincible = true;

        m_meshTransform.DOKill();
        m_meshTransform.DOScale(m_startingScale, m_toadSettings.m_swellUpOver).SetEase(Ease.OutElastic);
        // m_toadRenderer.material = m_normalMaterial;
        m_material.SetTexture("_MainTex", m_normalTexture);

        StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_Cutoff", 1.1f, 0.8f, -0.15f, false));
        StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_material, "_FresnelStrength", 20.0f, 5.0f, -7.5f, false));

        MessageBus.TriggerEvent(EMessageType.vulnerableEnd);
    }

    
}
