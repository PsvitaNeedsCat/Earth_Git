using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestListener : MonoBehaviour
{
    UnityAction audioAction;

    private void Awake()
    {
        audioAction = new UnityAction(PlayAudio);

        MessageBus.AddListener(EMessageType.PlayAudio, audioAction);
    }

    private void PlayAudio()
    {
        Debug.Log(name + ": PLAYING AUDIO");
    }

    private void OnDestroy()
    {
        MessageBus.RemoveListener(EMessageType.PlayAudio, audioAction);
    }
}
