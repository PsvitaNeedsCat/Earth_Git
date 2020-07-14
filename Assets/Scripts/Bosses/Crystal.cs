using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Crystal : MonoBehaviour
{
    public int m_setMaxHealth = 3;
    [SerializeField] private bool m_endgameCrystal = false;
    [SerializeField] private eChunkEffect m_crystalType;
    [SerializeField] private string m_nextSceneName;
    private bool m_collected = false;

    public void AETingSound() => MessageBus.TriggerEvent(EMessageType.ting);

    public void AEAnimationFinished() => Destroy(GetComponent<Animator>());

    private void OnTriggerEnter(Collider other)
    {
        // Hit player
        Player player = other.GetComponent<Player>();
        if (player && !m_collected)
        {
            m_collected = true;

            // If it is the endgame crystal, reset the powers, otherwise unlock one
            if (!m_endgameCrystal) { player.PowerUnlocked(m_crystalType); }
            else
            {
                MainMenu menuObj = FindObjectOfType<MainMenu>();
                if (menuObj) { Destroy(menuObj.gameObject); }
                player.ResetPowers(); 
            }

            player.GetComponent<PlayerController>().SetMaxHealth(m_setMaxHealth);
            PlayerController.m_saveOnAwake = true;

            transform.DOMove(transform.position + Vector3.up, 1.0f).SetEase(Ease.OutSine);
            transform.DORotate(new Vector3(0.0f, 720.0f, 0.0f), 1.0f, RotateMode.LocalAxisAdd);
            RoomManager.Instance.LoadScene(m_nextSceneName);
        }
    }
}
