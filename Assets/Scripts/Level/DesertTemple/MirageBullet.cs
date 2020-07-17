using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageBullet : MirageParent
{
    [SerializeField] private Material[] m_materials = new Material[] { };
    private GlobalEnemySettings m_settings;

    protected override void Awake()
    {
        // Base
        base.Awake();

        m_settings = Resources.Load<GlobalEnemySettings>("ScriptableObjects/GlobalEnemySettings");

        // Set velocity
        GetComponent<Rigidbody>().velocity = transform.forward * m_settings.m_mirageBulletSpeed;
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

    public void Init(eChunkEffect _bulletEffect, eChunkEffect _playerEffect)
    {
        // Update bullet's effect
        m_effectType = _bulletEffect;

        // Update the material
        GetComponent<MeshRenderer>().material = m_materials[(int)m_effectType];

        // Update the current power
        PowerChanged(_playerEffect.ToString());
    }
}
