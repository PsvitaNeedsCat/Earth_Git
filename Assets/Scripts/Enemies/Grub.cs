using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Grub : MonoBehaviour
{
    [Tooltip("How many tiles forward the end position is compared to the grub")]
    [SerializeField] private int m_endDistance = 2;
    private int m_moveCount = 0;

    private GlobalEnemySettings m_settings;
    private float m_moveTimer = 0.0f;
    private bool m_moving = false;
    [SerializeField] private Transform m_projSpawn;
    [SerializeField] private GameObject m_projPrefab;

    // Position data
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private Vector3 m_currentTarget;

    private bool m_dead = false;

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        m_moveTimer = m_settings.m_grubMaxMoveTime;

        m_startPos = transform.position;
        m_endPos = transform.position;
        m_endPos += transform.forward * m_endDistance;
        m_currentTarget = m_endPos;
    }

    private void FixedUpdate()
    {
        if (m_dead) { return; }

        if (m_moveTimer <= 0.0f)
        {
            m_moveTimer = m_settings.m_grubMaxMoveTime;
            // Move
            Move();
        }
        else if (!m_moving) { m_moveTimer -= Time.deltaTime; }
    }

    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk)
        {
            // Kill grub
            if (chunk.m_currentEffect == eChunkEffect.waterTrail)
            {
                m_dead = true;
            }
            
            Destroy(other.gameObject);

            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            player.KnockBack((player.transform.position - transform.position).normalized);
            player.GetComponent<HealthComponent>().Health -= 1;
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

    private void Move()
    {
        m_moving = true;

        // Reached end point
        if (m_moveCount >= m_endDistance)
        {
            FireProjectile();
            TurnAround();
            m_moveCount = 0;
        }

        // Move forward one tile
        Vector3 movePos = transform.position;
        movePos += transform.forward;
        transform.DOMove(movePos, m_settings.m_grubSpeed).OnComplete(() => m_moving = false);

        m_moveCount += 1;
    }

    private void TurnAround()
    {
        transform.Rotate(Vector3.up, 180.0f);
        m_projSpawn.RotateAround(transform.position, Vector3.up, 180.0f);
        m_currentTarget = (m_currentTarget == m_startPos) ? m_endPos : m_startPos;
        m_moveTimer = m_settings.m_grubMaxMoveTime;
    }
}
