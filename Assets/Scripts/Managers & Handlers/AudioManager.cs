using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class AudioManager : MonoBehaviour
{
    // Private variables
    private static AudioManager s_instance = null;
    public static AudioManager Instance
    {
        get 
        {
            return s_instance; 
        }
    }
    private string m_soundEffectsPath = "Audio";
    private Dictionary<string, AudioClip> m_soundDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject); 
        }
        else 
        {
            s_instance = this; 
        }
        
        // Add the audio clips to a dictionary with their name as the key
        AudioClip[] audioClips = Resources.LoadAll(m_soundEffectsPath, typeof(AudioClip)).Cast<AudioClip>().ToArray();
        for (int i = 0; i < audioClips.Length; i++)
        {
            char[] name = audioClips[i].name.ToCharArray();
            name[0] = char.ToLower(name[0]);
            m_soundDictionary.Add(name.ArrayToString(), audioClips[i]);
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

    // Called by the message bus - cannot have a second parameter
    public void PlaySoundVaried(string _soundName)
    {
        PlaySound(_soundName, true);
    }

    // Plays a sound with the option of being varied slightly
    public void PlaySound(string _soundName, bool _varied = false)
    {
        AudioClip clip = m_soundDictionary[_soundName];

        if (!clip)
        {
            Debug.LogError("Sound effect could not be found with name: " + _soundName);
            return;
        }

        GameObject soundEffectPlayer = new GameObject("SoundEffectPlayer");
        if (soundEffectPlayer)
        {
            soundEffectPlayer.transform.parent = transform;
            AudioSource audioSource = soundEffectPlayer.AddComponent<AudioSource>();
            if (_varied)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.volume = Random.Range(0.8f, 1.0f);
            }
            audioSource.PlayOneShot(clip);
            Destroy(soundEffectPlayer, clip.length);
        }
    }

    // Pauses all audio clips
    public void PauseAll()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<AudioSource>().Pause();
        }
    }

    // Unpauses all audio clips
    public void ContinuePlay()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<AudioSource>().UnPause();
        }
    }
}
