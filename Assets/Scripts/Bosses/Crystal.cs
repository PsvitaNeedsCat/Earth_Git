using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Crystal : MonoBehaviour
{
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

            FindObjectOfType<Player>().PowerUnlocked(eChunkEffect.water);

            transform.DOMove(transform.position + Vector3.up, 1.0f).SetEase(Ease.OutSine);
            transform.DORotate(new Vector3(0.0f, 720.0f, 0.0f), 1.0f, RotateMode.LocalAxisAdd);
            RoomManager.Instance.LoadScene(m_nextSceneName);
        }
    }
}
