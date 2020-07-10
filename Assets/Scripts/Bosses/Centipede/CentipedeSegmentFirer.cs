﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DG.Tweening;

public class CentipedeSegmentFirer : MonoBehaviour
{
    private GameObject m_projectilePrefab;

    private void Awake()
    {
        m_projectilePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Centipede/CentipedeBodyProjectile");
    }

    public void FireProjectiles(float _speed)
    {
        transform.DORewind();
        transform.DOPunchScale(Vector3.one * 0.2f, 0.15f);

        GameObject left = Instantiate(m_projectilePrefab, transform.position + -transform.right * 0.5f, Quaternion.identity, transform.parent.parent);
        GameObject right = Instantiate(m_projectilePrefab, transform.position + transform.right * 0.5f, Quaternion.identity, transform.parent.parent);

        left.GetComponent<Rigidbody>().velocity = -transform.right * _speed;
        right.GetComponent<Rigidbody>().velocity = transform.right * _speed;
    }
}