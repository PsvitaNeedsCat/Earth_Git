using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Key : MonoBehaviour
{
    public enum States
    {
        waiting,
        collected,
        unlocking,
        returning
    }
    [HideInInspector] public States m_state = States.waiting;
    public int m_keyID;
    [HideInInspector] public bool m_isLoaded = false;

    private Animator m_animator = null;
    private Player m_playerRef = null;
    private Vector3 m_animatorPrevLocalPosition = Vector3.zero;

    private float[] m_animationFrames = new float[]
    {
        0.0f,
        0.35f,
        0.75f
    };

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.checkKeyID, CheckKeyID);
        
        CheckKeyID("");
    }

    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.checkKeyID, CheckKeyID);
    }

    // Checks if the player has the current key using ID, if they do => destroy this key
    private void CheckKeyID(string _null)
    {
        if (m_isLoaded)
        {
            return;
        }

        if (!m_playerRef)
        {
            m_playerRef = FindObjectOfType<Player>();
        }

        foreach (int i in m_playerRef.m_collectedKeys)
        {
            if (i == m_keyID)
            {
                Destroy(gameObject);
                break;
            }
        }
    }

    private void Update()
    {
        if (m_state == States.collected && transform.position != m_playerRef.transform.position)
        {
            Vector3 newPos = m_playerRef.transform.position;
            newPos.y += 1.2f;

            transform.DOMove(newPos, 0.1f);
        }
    }

    // When the key collides with the player while not collected - collects the key
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (m_state == States.waiting && player)
        {
            m_playerRef = player;
            KeyCollected();
        }
    }

    // Removes the collider, and sets the key to float around the player's head
    private void KeyCollected()
    {
        if (!m_playerRef.m_collectedKeys.Contains(m_keyID))
        {
            m_playerRef.m_collectedKeys.Add(m_keyID);
        }
        transform.parent = null;
        FloatToPlayer();
    }

    // Called when the key is first collected - tweens the keys to the position where it will float around the player
    private void FloatToPlayer()
    {
        m_state = States.returning;
        
        Vector3 floatPosition = m_playerRef.transform.position;
        floatPosition.y += 1.2f;

        Destroy(GetComponent<Collider>());

        transform.DOLocalMove(floatPosition, 0.5f).OnComplete(() => BeginToFloat());
    }

    // Called once the key is collected - makes the key float around the player's head
    private void BeginToFloat()
    {
        m_state = States.collected;

        m_animator.SetTrigger("Float");

        float animationFrame = m_animationFrames[m_playerRef.m_collectedKeys.Count - 1];

        m_animator.Play("Floating", 0, animationFrame);
    }

    // Called by the door - pauses the floating animation
    public void PauseAnimation()
    {
        m_animator.enabled = false;
        m_animatorPrevLocalPosition = m_animator.transform.localPosition;
        m_animator.transform.localPosition = Vector3.zero;
        m_animator.transform.localRotation = Quaternion.identity;
        m_state = States.unlocking;
    }

    // Called by the door - continues the animation of floating around the player
    public void ContinueAnimation()
    {
        m_animator.transform.localPosition = m_animatorPrevLocalPosition;
        m_animator.enabled = true;
        m_state = States.collected;
    }
}
