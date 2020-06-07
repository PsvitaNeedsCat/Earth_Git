using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    // Private variables
    private static AudioManager m_instance = null;
    public static AudioManager Instance { get { return m_instance; } }
    private string m_soundEffectsPath = "Audio";
    private Dictionary<string, AudioClip> m_soundDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (m_instance != null && m_instance != this) { Destroy(this.gameObject); }
        else { m_instance = this; }
        
        AudioClip[] audioClips = Resources.LoadAll(m_soundEffectsPath, typeof(AudioClip)).Cast<AudioClip>().ToArray();

        for (int i = 0; i < audioClips.Length; i++)
        {
            m_soundDictionary.Add(audioClips[i].name, audioClips[i]);
        }
    }

    private void OnEnable()
    {
        // Add sounds to message bus
        foreach (KeyValuePair<string, AudioClip> i in m_soundDictionary)
        {
            EMessageType listenerEnum = EMessageType.none;
            System.Enum.TryParse(i.Key, out listenerEnum);
            MessageBus.AddListener(listenerEnum, PlaySoundVaried);
        }
    }

    private void OnDisable()
    {
        // Remove all sounds in the message bus
        foreach (KeyValuePair<string, AudioClip> i in m_soundDictionary)
        {
            EMessageType listenerEnum = EMessageType.none;
            Debug.Assert(System.Enum.TryParse(i.Key, out listenerEnum), "Audio clip called " + i.Key + " is not a valid EMessageType");
            MessageBus.RemoveListener(listenerEnum, PlaySoundVaried);
        }
    }

    public void PlaySoundVaried(string soundName)
    {
        AudioClip clip = m_soundDictionary[soundName];
        
        if (!clip)
        {
            Debug.LogError("Sound effect could not be found with name: " + soundName);
            return;
        }

        GameObject soundEffectPlayer = new GameObject("SoundEffectPlayer");
        if (soundEffectPlayer)
        {
            soundEffectPlayer.transform.parent = this.transform;
            AudioSource audioSource = soundEffectPlayer.AddComponent<AudioSource>();
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.volume = Random.Range(0.8f, 1.0f);
            audioSource.PlayOneShot(clip);
            Destroy(soundEffectPlayer, clip.length);
        }
    }
}
