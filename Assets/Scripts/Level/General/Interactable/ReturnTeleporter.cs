using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTeleporter : Interactable
{
    [Tooltip("The name of the room where the player will be teleported")]
    [SerializeField] private string m_destinationRoom = "";
    private bool m_unlocked = false;

    [SerializeField] private ParticleSystem m_particles;
    [SerializeField] private Material m_activeMaterial = null;

    private Vector3 m_startPosition;
    public GameObject m_crystalMesh;
    public float m_frequency;
    public float m_amplitude;
    public float m_rotationSpeed;

    public override void Awake()
    {
        base.Awake();

        m_startPosition = m_crystalMesh.transform.position;
    }

    // Stops the player moving and teleports them back to a specified room
    public override void Invoke()
    {
        if (!m_unlocked)
        {
            return;
        }

        MessageBus.TriggerEvent(EMessageType.crystalHealed);

        PlayerInput player = FindObjectOfType<PlayerInput>();

        player.SetMovement(false);
        player.GetComponent<HealthComponent>().SetInvincibleTimer(2.0f);

        FindObjectOfType<RoomManager>().PrepareToChangeRoom(m_destinationRoom, false);
    }

    // Stop prompt from appearing if locked
    public override void Update()
    {
        if (m_unlocked)
        {
            base.Update();

            m_crystalMesh.transform.position = m_startPosition + Vector3.up * (m_amplitude * Mathf.Sin(Time.time * m_frequency) - (m_amplitude / 2.0f));
            m_crystalMesh.transform.rotation = Quaternion.Euler(0.0f, m_rotationSpeed * Time.deltaTime, 0.0f) * m_crystalMesh.transform.rotation;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (m_unlocked)
        {
            m_particles.Play();
        }
    }

    // Unlocks the teleport for use
    public void Unlock()
    {
        m_unlocked = true;

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = m_activeMaterial;
        }

        m_particles.Play();
    }
}
