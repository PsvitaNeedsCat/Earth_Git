using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private EMessageType m_startingMusic = EMessageType.none;

    private static MusicManager s_instance = null;

    private string m_musicPath = "Music";
    private Dictionary<string, AudioClip> m_musicDictionary = new Dictionary<string, AudioClip>();
    private AudioSource m_audioSource;

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

        // Get music
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(m_musicPath);

        for (int i = 0; i < audioClips.Length; i++)
        {
            m_musicDictionary.Add(audioClips[i].name, audioClips[i]);
        }

        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.loop = true;
    }

    private void OnEnable()
    {
        // Add sounds to message bus
        foreach (KeyValuePair<string, AudioClip> i in m_musicDictionary)
        {
            EMessageType listenerEnum = EMessageType.none;
            System.Enum.TryParse(i.Key, out listenerEnum);
            MessageBus.AddListener(listenerEnum, PlayMusic);
        }

        MessageBus.AddListener(EMessageType.stopMusic, StopMusic);
    }

    private void OnDisable()
    {
        // Remove all sounds in the message bus
        foreach (KeyValuePair<string, AudioClip> i in m_musicDictionary)
        {
            EMessageType listenerEnum = EMessageType.none;
            Debug.Assert(System.Enum.TryParse(i.Key, out listenerEnum), "Audio clip called " + i.Key + " is not a valid EMessageType");
            MessageBus.RemoveListener(listenerEnum, PlayMusic);
        }

        MessageBus.RemoveListener(EMessageType.stopMusic, StopMusic);
    }

    private void Start()
    {
        if (m_startingMusic != EMessageType.none)
        {
            PlayMusic(m_startingMusic.ToString());
        }
    }

    private void PlayMusic(string _name)
    {
        if (m_audioSource.isPlaying)
        {
            // If the clip playing is the one wanted to be played, return
            if (m_audioSource.clip.name == _name)
            {
                return; 
            }

            // If the clip playing is not the one wanted, stop it playing
            m_audioSource.Stop();
        }

        m_audioSource.clip = m_musicDictionary[_name];

        m_audioSource.Play();
    }

    private void StopMusic(string _null)
    {
        m_audioSource.Stop();
    }
    
    // Fades the music out over a duration
    public IEnumerator FadeMusicOut(float _duration)
    {
        float timer = 0.0f;
        float startVolume = m_audioSource.volume;

        while (timer < _duration)
        {
            timer += Time.deltaTime;
            m_audioSource.volume = Mathf.Lerp(startVolume, 0.0f, timer / _duration);
            yield return null;
        }

        m_audioSource.Stop();
        m_audioSource.volume = startVolume;
    }
}
