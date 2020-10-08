using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DG.Tweening;

public class CentipedeSegmentFirer : MonoBehaviour
{
    public ParticleSystem m_fireEffects;
    private GameObject m_projectilePrefab;
    private CentipedeSettings m_settings;

    private void Awake()
    {
        m_projectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Centipede/CentipedeBodyProjectile");
        m_settings = Resources.Load<CentipedeSettings>("ScriptableObjects/CentipedeBossSettings");
    }

    // Fire a projectile left and right
    public void FireProjectiles(float _speed)
    {
        m_fireEffects.Play();
        MessageBus.TriggerEvent(EMessageType.centipedeBodyFire);

        // Swell up as firing
        transform.DORewind();
        transform.DOPunchScale(Vector3.one * 0.2f, 0.15f);

        Vector3 heightOffset = Vector3.up * m_settings.m_heightOffset;
        GameObject left = Instantiate(m_projectilePrefab, transform.position + -transform.right * 0.5f + heightOffset, Quaternion.identity, transform.parent.parent);
        GameObject right = Instantiate(m_projectilePrefab, transform.position + transform.right * 0.5f + heightOffset, Quaternion.identity, transform.parent.parent);

        left.transform.LookAt(left.transform.position + -transform.right);
        right.transform.LookAt(right.transform.position + transform.right);

        left.GetComponent<Rigidbody>().velocity = -transform.right * _speed;
        right.GetComponent<Rigidbody>().velocity = transform.right * _speed;
    }
}
