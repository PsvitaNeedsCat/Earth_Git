using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;

public class Crystal : MonoBehaviour
{
    [SerializeField] private int m_setMaxHealth = 3;
    [SerializeField] private EChunkEffect m_crystalType;
    [SerializeField] private GameObject[] m_effects = new GameObject[] { };
    private bool m_collected = false;
    private Dialogue m_dialogue;

    private void Awake()
    {
        m_dialogue = GetComponent<Dialogue>();
    }

    public void AETingSound()
    {
        MessageBus.TriggerEvent(EMessageType.ting);
    }

    public void AEAnimationFinished()
    {
        Destroy(GetComponent<Animator>());
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hit player
        Player player = other.GetComponent<Player>();
        if (player && !m_collected)
        {
            Collected(player);
        }
    }

    // Called when the player collects the crystal
    private void Collected(Player _player)
    {
        StartCoroutine(FindObjectOfType<MusicManager>().FadeMusicOut(1.0f));
        MessageBus.TriggerEvent(EMessageType.crystalCollected);

        m_collected = true;

        // If it is the endgame crystal, reset the powers, otherwise unlock one
        _player.PowerUnlocked(m_crystalType);

        _player.GetComponent<PlayerController>().SetMaxHealth(m_setMaxHealth);
        PlayerController.s_saveOnAwake = true;

        // Move to camera
        Vector3 tweenPos = Camera.main.transform.position;
        Vector3 camForward = Camera.main.transform.forward;
        camForward = Quaternion.AngleAxis(-10.0f, Camera.main.transform.right) * camForward;
        tweenPos += camForward * 2.0f;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(tweenPos, 1.0f));
        seq.Insert(0.0f, transform.DOScale(Camera.main.orthographicSize * 0.01f, 1.0f));
        seq.Play();
        // Rotate
        transform.DORotate(new Vector3(0.0f, 360.0f, 0.0f), 3.0f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);

        // Init dialogue
        if (m_dialogue)
        {
            m_dialogue.Invoke();
        }

        // Remove effects
        for (int i = 0; i < m_effects.Length; i++)
        {
            m_effects[i].SetActive(false);
        }
    }
}
