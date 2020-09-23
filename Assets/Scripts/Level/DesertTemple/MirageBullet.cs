using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageBullet : MirageParent
{
    [SerializeField] private Material[] m_materials = new Material[] { };
    private GlobalEnemySettings m_settings;
    private const float m_lifeTime = 5.0f;
    private float m_lifeTimer = 0.0f;

    protected override void Awake()
    {
        // Base
        base.Awake();

        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        // Set velocity
        GetComponent<Rigidbody>().velocity = transform.forward * m_settings.m_mirageBulletSpeed;
    }

    private void Update()
    {
        if (m_lifeTimer >= m_lifeTime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If hit the player
        Player player = collision.collider.GetComponent<Player>();
        if (player)
        {
            player.GetComponent<HealthComponent>().Health -= 1;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If hit wall
        if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            Destroy(this.gameObject);
        }
    }

    public void Init(EChunkEffect _bulletEffect, EChunkEffect _playerEffect)
    {
        // Update bullet's effect
        m_effectType = _bulletEffect;

        // Update the material
        // GetComponent<MeshRenderer>().material = m_materials[(int)m_effectType];

        // Update the current power
        PowerChanged(_playerEffect.ToString());
    }
}
