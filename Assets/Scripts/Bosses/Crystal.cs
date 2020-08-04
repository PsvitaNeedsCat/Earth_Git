using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;

public class Crystal : MonoBehaviour
{
    [SerializeField] private int m_setMaxHealth = 3;
    [SerializeField] private EChunkEffect m_crystalType;
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
            m_collected = true;

            // If it is the endgame crystal, reset the powers, otherwise unlock one
            player.PowerUnlocked(m_crystalType);

            player.GetComponent<PlayerController>().SetMaxHealth(m_setMaxHealth);
            PlayerController.s_saveOnAwake = true;

            transform.DOMove(transform.position + Vector3.up, 1.0f).SetEase(Ease.OutSine);
            transform.DORotate(new Vector3(0.0f, 720.0f, 0.0f), 1.0f, RotateMode.LocalAxisAdd);

            // Init dialogue
            m_dialogue.Invoke();
        }
    }
}
