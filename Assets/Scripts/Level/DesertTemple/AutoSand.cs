using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSand : MonoBehaviour
{
    private SandBlock m_sandChild = null;
    [Tooltip("Leave null for default sand")]
    [SerializeField] private GameObject m_sandPrefab = null;
    [SerializeField] private float m_respawnHeight = 10.0f;

    private void Awake()
    {
        m_sandChild = GetComponentInChildren<SandBlock>();

        if (!m_sandPrefab)
        {
            m_sandPrefab = Resources.Load<GameObject>("Prefabs/Sand Block");
        }
    }

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.glassDestroyed, SandChildDestroyed);
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.glassDestroyed, SandChildDestroyed);
    }

    // Checks if the sand object destroyed was it's own, then spawns a new one
    private void SandChildDestroyed(string _null)
    {
        if (m_sandChild != null && !m_sandChild.m_isDestroyed)
        {
            return;
        }

        // Spawn new sand at the specified height and make it fall
        Vector3 sandSpawn = transform.position;
        sandSpawn.y += m_respawnHeight;
        m_sandChild = Instantiate(m_sandPrefab, sandSpawn, Quaternion.identity).GetComponent<SandBlock>();
        m_sandChild.transform.parent = transform;
        m_sandChild.Fall();
    }
}
