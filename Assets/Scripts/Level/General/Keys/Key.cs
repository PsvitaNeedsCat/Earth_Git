using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Key : MonoBehaviour
{
    public enum Type
    {
        basic,
        waterBoss,
        fireBoss,
        sandBoss
    }
    public Type m_type = Type.basic;
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
    private DoorManager m_doorManager = null;

    private Animator m_animator = null;
    private Player m_playerRef = null;

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_doorManager = FindObjectOfType<DoorManager>();
    }

    private void Start()
    {
        if (m_doorManager)
        {
            m_doorManager.AddKey(m_keyID);

            if (!m_isLoaded && m_doorManager.HasKeyBeenCollected(m_keyID))
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        FindObjectOfType<KeyUI>().KeyRemoved(m_type);
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
        m_beltLocation = m_playerRef.GetFreeBeltSlot();
        if (!m_beltLocation)
        {
            return;
        }

        if (!m_playerRef.m_collectedKeys.Contains(m_keyID))
        {
            m_playerRef.m_collectedKeys.Add(m_keyID);
        }
        
        FindObjectOfType<KeyUI>().KeyCollected(m_type);

        if (!m_isLoaded)
        {
            MessageBus.TriggerEvent(EMessageType.keyCollected);
            MessageBus.TriggerEvent(EMessageType.keySpawned);
        }

        m_doorManager.CollectedKey(m_keyID);

        FloatToPlayer();
    }

    // Called when the key is first collected - tweens the keys to the position where it will float around the player
    private void FloatToPlayer()
    {
        m_state = States.returning;

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
