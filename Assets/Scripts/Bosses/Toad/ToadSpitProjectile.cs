using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ToadSpitProjectile : MonoBehaviour
{
    public LayerMask m_dropShadowLayerMask;
    public float m_dropShadowMaxRange = 70.0f;
    public float m_dropShadowMinScale = 0.01f;
    public float m_dropShadowMaxScale = 0.1f;
    public GameObject m_dropShadow;

    bool m_isFragment = false;
    [HideInInspector] public bool m_shouldSplit = false;
    [HideInInspector] public Tile m_aimedTile;
    [HideInInspector] public Rigidbody m_rigidbody;

    readonly int m_damage = 1;

    // Also determines number of fragments
    readonly Vector3[] m_fragmentDirections = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
    readonly float m_fragmentLifetime = 3.0f;
    readonly float m_fragmentHeightGain = 1.0f;
    readonly float m_gravity = 100.0f;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateDropShadow();        
    }

    private void FixedUpdate()
    {
        // Apply gravity
        m_rigidbody.AddForce(Vector3.down * m_gravity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If allowed, split
        if (m_shouldSplit && !m_isFragment) Split();

        HealthComponent playerHealthComp = other.GetComponent<HealthComponent>();

        // If the player is hit
        if (playerHealthComp && other.GetComponent<PlayerController>())
        {
            // Damage the player
            playerHealthComp.Health -= 1;
        }

        // If not a fragment, return aimed tile to the spit attack's dictionary
        if (!m_isFragment) ToadSpit.ProjectileDestroyed(m_aimedTile);

        MessageBus.TriggerEvent(EMessageType.projectileSplash);
        Destroy(this.gameObject);
    }

    private void Split()
    {
        // Spawn four fragments around this one
        for (int i = 0; i < m_fragmentDirections.Length; i++)
        {
            ToadSpitProjectile fragment = Instantiate(this.gameObject, transform.position + m_fragmentDirections[i] + Vector3.up * m_fragmentHeightGain,
                transform.rotation, null).GetComponent<ToadSpitProjectile>();
            fragment.m_isFragment = true;
        }
    }

    private void UpdateDropShadow()
    {
        RaycastHit hitInfo;

        // If there is a surface below
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_dropShadowMaxRange, m_dropShadowLayerMask))
        {
            // If surface is out of range, deactivate drop shadow
            if (hitInfo.distance > m_dropShadowMaxRange || hitInfo.distance < 0.0f)
            {
                m_dropShadow.SetActive(false);
                return;
            }

            // Otherwise, scale drop shadow based on distance, and place it just above floor
            float newScale = Mathf.Lerp(m_dropShadowMinScale, m_dropShadowMaxScale, (m_dropShadowMaxRange - hitInfo.distance) / m_dropShadowMaxRange);
            m_dropShadow.SetActive(true);
            m_dropShadow.transform.localScale = Vector3.one * newScale;
            m_dropShadow.transform.position = hitInfo.point + Vector3.up * 0.01f;
        }
        else
        {
            m_dropShadow.SetActive(false);
        }
    }
}
