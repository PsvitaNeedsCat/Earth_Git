using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    // Public variables
    [HideInInspector] public EChunkEffect m_effect = EChunkEffect.none;

    // Private variables
    private int m_framesSkipped = 0;
    private GlobalPlayerSettings m_settings;
    private Vector3 m_playerPos;

    // Called when hurtbox is instantiated
    public void Init(Vector3 _pos, EChunkEffect _effect)
    {
        m_playerPos = _pos;
        m_effect = _effect;
    }

    private void Awake()
    {
        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");

        // Increase size if using fire punch
        if (Player.s_currentEffect == EChunkEffect.fire)
        {
            Vector3 newScale = transform.localScale;
            newScale.z = m_settings.m_firePunchSize;
            transform.localScale = newScale;
        }
    }

    private void Update()
    {
        // Check the amount of frames skipped
        if (m_framesSkipped < m_settings.m_framesBeforeDestroy)
        {
            m_framesSkipped++; 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponentInParent<Chunk>();

        // Check collision is a chunk
        if (chunk)
        {
            Vector3 hitDir = chunk.transform.position - m_playerPos;
            hitDir.y = 0.0f;
            hitDir.Normalize();

            Vector3 cardinal = hitDir.Cardinal();
            chunk.Hit(cardinal * m_settings.m_chunkHitForce, m_effect);

            ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.small);

            Destroy(gameObject);
        }

        WallButton button = other.GetComponent<WallButton>();
        if (button)
        {
            button.Invoke(); 
        }
    }
}
