using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;

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
    [SerializeField] private UnityEvent m_collectedEvent = new UnityEvent();
    [SerializeField] private ParticleSystem[] m_particles = new ParticleSystem[] { };
    private DoorManager m_doorManager = null;

    private Animator m_animator = null;
    private Player m_playerRef = null;
    private PlayerInput m_playerInputRef = null;

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
                m_collectedEvent.Invoke();
            }
        }
    }

    // Called by door master - removes the key visually
    public void RemoveFromUI()
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
            m_playerInputRef = player.GetComponent<PlayerInput>();
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


        m_doorManager.CollectedKey(m_keyID);

        if (m_isLoaded)
        {
            StartCoroutine(KeyCollectAnimation(true));
            return;
        }

        m_playerInputRef.SetMovement(false);
        m_playerInputRef.SetCombat(false);

        StartCoroutine(KeyCollectAnimation());
    }

    private IEnumerator KeyCollectAnimation(bool _silent = false)
    {
        MusicManager musicManager = FindObjectOfType<MusicManager>();

        if (!_silent)
        {
            MessageBus.TriggerEvent(EMessageType.keySpawned);

            StartCoroutine(musicManager.FadeMusic(1.5f));
        }

        // Disable effects
        Destroy(GetComponent<Collider>());
        for (int i = 0; i < m_particles.Length; i++)
        {
            m_particles[i].Stop();
            Destroy(m_particles[i]);
        }

        bool animationCompleted = false;

        if (!_silent)
        {
            // Animate
            Sequence animation = DOTween.Sequence();
            Vector3 rotation = new Vector3(0.0f, 360.0f, 0.0f);
            float moveY = transform.position.y + 1.0f;

            animation.Append(transform.DORotate(rotation, 1.0f, RotateMode.LocalAxisAdd));
            animation.Insert(0.0f, transform.DOMoveY(moveY, 1.0f));
            animation.OnComplete(() => animationCompleted = true);
            animation.Play();

            while (!animationCompleted)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            m_playerInputRef.SetCombat(true);
            m_playerInputRef.SetMovement(true);

            MessageBus.TriggerEvent(EMessageType.keyCollected);

            StartCoroutine(musicManager.FadeMusic(1.0f, false));
        }

        FloatToPlayer();

        m_collectedEvent.Invoke();
    }

    // Called when the key is first collected - tweens the keys to the position where it will float around the player
    private void FloatToPlayer()
    {
        m_state = States.returning;

        transform.parent = m_beltLocation.transform;

        m_animator.transform.localPosition = Vector3.zero;
        m_animator.transform.localRotation = Quaternion.identity;
        Destroy(m_animator);

        transform.DOScale(0.1f, 0.4f);
        transform.DOLocalRotateQuaternion(Quaternion.identity, 0.4f);
        transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => m_state = States.collected);
    }
}
