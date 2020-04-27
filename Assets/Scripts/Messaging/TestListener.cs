using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class TestListener : MonoBehaviour
{   
    System.Action<string> audioAction;
    
    // If this was audio manager
    // Dictionary here of audio clips
    // Audio clips all loaded in from resources

    private void Awake()
    {
        audioAction = new System.Action<string>(PlayAudio);

        // Add a listener to each message type we need to play audio for
        MessageBus.AddListener(EMessageType.PlayerPunch, audioAction);
    }

    private void PlayAudio(string _clipName)
    {
        // m_audioClips[_clipName].PlayClip() or whatever

        Debug.Log("Playing audio: " + _clipName);
        // 
    }

    private void OnDestroy()
    {
        MessageBus.RemoveListener(EMessageType.PlayerPunch, audioAction);
    }
}
