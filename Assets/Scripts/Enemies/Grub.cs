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
    private bool m_invincible = true;

    [SerializeField] private MeshRenderer m_renderer;
    [SerializeField] private Material[] m_defaultMats;

    // Position data
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private Vector3 m_currentTarget;
    private float m_tweenMat = 0.0f;

    private bool m_dead = false;

    private void OnEnable()
    {
        CheckForStationaryProjectiles();
    }

    private void Awake()
    {
        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        m_moveTimer = m_settings.m_grubMaxMoveTime;

        m_startPos = transform.position;
        m_endPos = transform.position;
        m_endPos += transform.forward * m_endDistance;
        m_currentTarget = m_endPos;

        for (int i = 0; i < m_renderer.materials.Length; i++)
        {
            m_renderer.materials[i].SetFloat("_Blend", 0.0f);
        }
    }

    private void FixedUpdate()
    {
        if (m_dead) { return; }

        if (m_moveTimer <= 0.0f && m_invincible)
        {
            m_moveTimer = m_settings.m_grubMaxMoveTime;
            // Move
            Move();
        }
        else if (!m_moving) { m_moveTimer -= Time.deltaTime; }
    }

    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponentInParent<Chunk>();
        if (chunk)
        {
            // Kill grub
            if (chunk.m_currentEffect == EChunkEffect.water)
            {
                MessageBus.TriggerEvent(EMessageType.waterChunkDestroyed);

                if (!m_dead && !m_invincible) { Dead(); }
            }
            else
            {
                MessageBus.TriggerEvent(EMessageType.chunkDestroyed);
            }

            Destroy(chunk.gameObject);

            return;
        }

        if (m_dead || m_invincible) { return; }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            player.KnockBack((player.transform.position - transform.position).normalized);
            player.GetComponent<HealthComponent>().Health -= 1;
        }
    }

    private void Dead()
    {
        transform.DOKill();
        transform.DOScale(0.1f, 0.2f).OnComplete(() => Destroy(transform.parent.gameObject));
        m_dead = true;
        MessageBus.TriggerEvent(EMessageType.grubKilled);
        MessageBus.TriggerEvent(EMessageType.lavaToStone);
    }

    private void ChargeUp()
    {
        m_invincible = false;

        transform.DOScale(m_settings.m_grubGrowSize, m_settings.m_grubVulnerableTime).OnComplete(() => FireProjectile());
        for (int i = 0; i < m_renderer.materials.Length; i++)
        {
            m_renderer.materials[i].SetFloat("_Blend", 1.0f);
        }
    }

    private void FireProjectile()
    {
        // MessageBus.TriggerEvent(EMessageType.enemySpit);
        MessageBus.TriggerEvent(EMessageType.grubFire);

        transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutBounce);
        for (int i = 0; i < m_renderer.materials.Length; i++)
        {
            m_renderer.materials[i].SetFloat("_Blend", 0.0f);
        }
        m_invincible = true;

        // Init
        Projectile proj = Instantiate(m_projPrefab, m_projSpawn.position, m_projSpawn.rotation).GetComponent<Projectile>();
        proj.transform.parent = transform.parent;
        proj.Init(m_settings.m_grubProjDamage);
        proj.GetComponent<Rigidbody>().AddForce(proj.transform.forward * m_settings.m_grubProjSpeed, ForceMode.Impulse);
        proj.transform.parent = transform.parent;

        // Tween
        Vector3 projScale = proj.transform.localScale;
        proj.transform.localScale = projScale * 0.1f;
        proj.transform.DOScale(projScale, 0.5f).SetEase(Ease.OutElastic);

        m_moving = false;
    }

    private void Move()
    {
        m_moving = true;

        // Reached end point
        if (m_moveCount >= m_endDistance)
        {
            m_moveCount = 0;
            TurnAround();
        }
        else
        {
            // Move forward one tile
            Vector3 movePos = transform.position;
            movePos += transform.forward;
            transform.DOMove(movePos, m_settings.m_grubSpeed).OnComplete(() => { ChargeUp(); m_moveCount += 1; });
        }
    }

    private void TurnAround()
    {
        transform.Rotate(Vector3.up, 180.0f);
        m_projSpawn.RotateAround(transform.position, Vector3.up, 180.0f);
        m_currentTarget = (m_currentTarget == m_startPos) ? m_endPos : m_startPos;
        m_moveTimer = m_settings.m_grubMaxMoveTime;
        m_moving = false;
    }

    // Checks if 'GrubParent' has any stationary projectiles - if so, give them force
    private void CheckForStationaryProjectiles()
    {
        Projectile[] projectiles = transform.parent.GetComponentsInChildren<Projectile>();

        for (int i = 0; i < projectiles.Length; i++)
        {
            Rigidbody rigidBody = projectiles[i].GetComponent<Rigidbody>();
            if (rigidBody.velocity.magnitude <= Vector3.zero.magnitude)
            {
                rigidBody.AddForce(projectiles[i].transform.forward * m_settings.m_grubProjSpeed, ForceMode.Impulse);
            }
        }
    }
}
