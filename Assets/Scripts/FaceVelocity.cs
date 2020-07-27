using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FaceVelocity : MonoBehaviour
{
    private Rigidbody m_rigidBody;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.LookAt(transform.position + m_rigidBody.velocity.normalized);
    }
}
