using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ToadSpitProjectile : MonoBehaviour
{
    public LayerMask m_dropShadowLayerMask;
    public float m_dropShadowMaxRange = 70.0f;
    public float m_dropShadowMinScale = 0.01f;
    public float m_dropShadowMaxScale = 0.1f;
    public GameObject m_dropShadow;

    [SerializeField] private GameObject m_visibleProjectile = null;
    private ToadBossSettings m_settings;

    bool m_isFragment = false;
    [HideInInspector] public bool m_shouldSplit = false;
    [HideInInspector] public Tile m_aimedTile;
    [HideInInspector] public Rigidbody m_rigidbody;
    [HideInInspector] public Vector3 m_fDir;

    // Also determines number of fragments
    readonly Vector3[] m_fragmentDirections = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
    readonly float m_fragmentLifetime = 5.0f;
    readonly float m_gravity = 100.0f;


    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_settings = Resources.Load<ToadBossSettings>("ScriptableObjects/ToadBossSettings");
    }

    private void Start()
    {
        if (m_isFragment)
        {
            Destroy(gameObject, m_fragmentLifetime);
        }
    }

    private void Update()
    {
        // Face velocity
        m_visibleProjectile.transform.up = -m_rigidbody.velocity;

        //UpdateDropShadow();
    }

    private void FixedUpdate()
    {
        // Apply gravity
        m_rigidbody.AddForce(Vector3.down * m_gravity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthComponent playerHealthComp = other.GetComponent<HealthComponent>();

        // If the player is hit
        if (playerHealthComp && other.GetComponent<PlayerController>())
        {
            // Damage the player
            playerHealthComp.Health -= 1;
        }

        // If not a fragment, return aimed tile to the spit attack's dictionary
        Vector3 effectScale = Vector3.one * 0.5f;
        if (!m_isFragment)
        {
            ToadSpit.ProjectileDestroyed(m_aimedTile);
            effectScale = Vector3.one;
        }

        // If allowed, split
        EffectsManager.EEffectType effectType = EffectsManager.EEffectType.waterProjectileDestroyed;
        if (m_shouldSplit && !m_isFragment)
        {
            Split();
            effectType = EffectsManager.EEffectType.rockToadProjectileDestroyed;
        }

        Quaternion rotation = Quaternion.Euler(new Vector3(270.0f, 0.0f, 0.0f));
        EffectsManager.SpawnEffect(effectType, transform.position, rotation, effectScale);

        MessageBus.TriggerEvent(EMessageType.projectileSplash);
        Destroy(gameObject);
    }

    private void Split()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Bosses/Toad/ToadSpitProjectile");

        // Spawn four fragments around this one
        for (int i = 0; i < m_fragmentDirections.Length; i++)
        {
            ToadSpitProjectile fragment = Instantiate(prefab, transform.position, transform.rotation).GetComponent<ToadSpitProjectile>();
            fragment.m_isFragment = true;
            fragment.m_fDir = m_fragmentDirections[i];
            Vector3 pos = fragment.transform.position;
            pos.y += 0.5f;
            fragment.transform.position = pos;
            fragment.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            fragment.transform.DOScale(0.5f, 0.07f).SetEase(Ease.InElastic);
            fragment.transform.parent = transform.parent.parent;

            // Projectile motion
            Vector3 force = Quaternion.Euler(m_fragmentDirections[i] * m_settings.m_fragmentAngle) * Vector3.up * m_settings.m_fragmentForce;
            fragment.m_rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}