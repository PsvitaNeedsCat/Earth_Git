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

    private Animator m_animator = null;
    private Player m_playerRef = null;
    private Vector3 m_animatorPrevLocalPosition = Vector3.zero;

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
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
            ++player.m_numKeys;
            FloatToPlayer();
        }
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
