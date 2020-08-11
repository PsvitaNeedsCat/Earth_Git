using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour
{
    public GameObject m_playerMesh;
    public Animator m_playerAnimator;
    public GameObject m_playerDropshadow;
    public Rigidbody m_pelvis;

    private Rigidbody m_playerRigidbody;
    private Collider m_playerCollider;

    private List<Collider> m_ragdollColliders;
    private Avatar m_avatar;

    private bool m_ragdollOn = false;

    // TEST
    private PlayerInput m_input;

    private void Awake()
    {
        m_avatar = m_playerAnimator.avatar;
    }

    private void Start()
    {
        m_input = GetComponent<PlayerInput>();
        SetupRagdoll();
    }

    private void SetupRagdoll()
    {
        m_playerRigidbody = GetComponent<Rigidbody>();
        m_playerCollider = GetComponent<Collider>();

        m_ragdollColliders = new List<Collider>(m_playerMesh.GetComponentsInChildren<Collider>());

        foreach(Collider collider in m_ragdollColliders)
        {
            collider.attachedRigidbody.maxDepenetrationVelocity = 1.0f;
        }

        SetRagdoll(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetRagdoll(!m_ragdollOn);
        }
    }

    

    public void SetRagdoll(bool _on)
    {
        Debug.Log("Set ragdoll to " + _on);
        m_ragdollOn = _on;

        if (m_ragdollOn)
        {
            Debug.LogError("haha");
            m_input.SetCombat(false);
            m_input.SetMovement(false);
            EnableRagdoll();

            m_pelvis.AddForce((Vector3.up + Vector3.forward + -Vector3.right) * 25.0f, ForceMode.Impulse);
        }
        else
        {
            DisableRagdoll();
            m_input.SetCombat(true);
            m_input.SetMovement(true);
        }
    }

    public void EnableRagdoll()
    {
        m_playerDropshadow.SetActive(false);

        m_playerAnimator.enabled = false;
        m_playerAnimator.avatar = null;

        m_playerRigidbody.isKinematic = true;
        m_playerCollider.enabled = false;
        m_playerRigidbody.velocity = Vector3.zero;        

        foreach (Collider collider in m_ragdollColliders)
        {
            collider.enabled = true;

            if (collider.attachedRigidbody != null)
            {
                collider.attachedRigidbody.isKinematic = false;
                collider.attachedRigidbody.velocity = Vector3.zero;
            }
        }
    }

    public void DisableRagdoll()
    {
        m_playerDropshadow.SetActive(true);

        foreach (Collider collider in m_ragdollColliders)
        {
            collider.enabled = false;

            if (collider.attachedRigidbody != null)
            {
                collider.attachedRigidbody.isKinematic = true;
            }
        }

        m_playerAnimator.avatar = m_avatar;
        m_playerAnimator.enabled = true;

        m_playerRigidbody.isKinematic = false;

        m_playerCollider.enabled = true;
    }
}
