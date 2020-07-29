using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FaceVelocity : MonoBehaviour
{
    public GameObject m_facer;

    private Rigidbody m_rigidBody;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_facer.transform.LookAt(m_facer.transform.position + m_rigidBody.velocity.normalized);
    }
}
