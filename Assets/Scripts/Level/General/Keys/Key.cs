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
    [HideInInspector] public GameObject m_beltLocation = null;

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

        FindObjectOfType<KeyUI>().UpdateIcons();

        if (!m_isLoaded)
        {
            MessageBus.TriggerEvent(EMessageType.keyCollected);
            MessageBus.TriggerEvent(EMessageType.keySpawned);
        }

        FloatToPlayer();
    }

    // Called when the key is first collected - tweens the keys to the position where it will float around the player
    private void FloatToPlayer()
    {
        m_state = States.returning;

        m_beltLocation = m_playerRef.m_keyBeltLocations[m_playerRef.m_collectedKeys.Count - 1].gameObject;
        transform.parent = m_beltLocation.transform;

        Destroy(GetComponent<Collider>());
        m_animator.transform.localPosition = Vector3.zero;
        m_animator.transform.localRotation = Quaternion.identity;
        Destroy(m_animator);

        transform.DOScale(0.1f, 0.4f);
        transform.DOLocalRotateQuaternion(Quaternion.identity, 0.4f);
        transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => m_state = States.collected);
    }
}
