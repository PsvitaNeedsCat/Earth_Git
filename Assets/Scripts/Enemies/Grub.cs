using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Grub : MonoBehaviour
{
    private GlobalEnemySettings m_settings;
    private Rigidbody m_rigidbody;
    private float m_moveTimer = 0.0f;
    private Vector3 m_startPos;
    [SerializeField] private Transform m_endTransform;
    private Vector3 m_currentTarget;
    [SerializeField] private Transform m_projSpawn;
    [SerializeField] private GameObject m_projPrefab;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");
        m_rigidbody = GetComponent<Rigidbody>();
        m_startPos = transform.position;
        m_currentTarget = m_endTransform.position;
    }

    private void FixedUpdate()
    {
        if (m_moveTimer <= 0.0f)
        {
            m_moveTimer = m_settings.m_grubMaxMoveTime;
            // Move
            m_rigidbody.AddForce(transform.forward * m_settings.m_grubMoveForce, ForceMode.Impulse);
        }
        else { m_moveTimer -= Time.deltaTime; }

        // Check distance
        float mag = (m_currentTarget - transform.position).magnitude;
        if (mag <= 0.1f)
        {
            // Grub has reached goal
            FireProjectile();
            transform.Rotate(Vector3.up, 180.0f);
            m_projSpawn.RotateAround(transform.position, Vector3.up, 180.0f);
            m_currentTarget = (m_currentTarget == m_startPos) ? m_endTransform.position : m_startPos;
            m_moveTimer = m_settings.m_grubMaxMoveTime;
        }
    }

    private void FireProjectile()
    {
        // Init
        Projectile proj = Instantiate(m_projPrefab, m_projSpawn.position, m_projSpawn.rotation).GetComponent<Projectile>();
        proj.Init(m_settings.m_grubProjDamage);
        proj.GetComponent<Rigidbody>().AddForce(proj.transform.forward * m_settings.m_grubProjSpeed, ForceMode.Impulse);

        // Tween
        Vector3 projScale = proj.transform.localScale;
        proj.transform.localScale = projScale * 0.1f;
        proj.transform.DOScale(projScale, 0.5f).SetEase(Ease.OutElastic);

        proj.transform.parent = RoomManager.Instance.GetActiveRoom().transform;
    }
}
