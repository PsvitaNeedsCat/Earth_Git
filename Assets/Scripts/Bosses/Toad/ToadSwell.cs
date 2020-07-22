using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToadSwell : ToadBehaviour
{
    public SkinnedMeshRenderer m_toadRenderer;
    public Material m_swollenMaterial;
    public Transform m_meshTransform;

    ToadBossSettings m_toadSettings;
    float m_swelledTimer = 0.0f;
    float m_startingScale;
    Material m_normalMaterial;
    HealthComponent m_toadHealth;

    private void Awake()
    {
        m_toadSettings = Resources.Load<ToadBossSettings>("ScriptableObjects/ToadBossSettings");
        m_startingScale = m_meshTransform.localScale.x;
        m_normalMaterial = m_toadRenderer.material;
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
        m_toadRenderer.material = m_swollenMaterial;
    }

    void SwellDown()
    {
        m_toadAnimator.SetTrigger("SwellDown");

        ToadBoss.s_tookDamage = false;
        m_toadHealth.IsInvincible = true;

        m_meshTransform.DOKill();
        m_meshTransform.DOScale(m_startingScale, m_toadSettings.m_swellUpOver).SetEase(Ease.OutElastic);
        m_toadRenderer.material = m_normalMaterial;
    }
}
