using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private EMessageType m_startingMusic = EMessageType.none;

    private static MusicManager s_instance = null;

    private string m_musicPath = "Music";
    private Dictionary<string, AudioClip> m_musicDictionary = new Dictionary<string, AudioClip>();
    public AudioSource m_audioSource;

    public float m_defaultVolume = 1.0f;

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
        m_defaultVolume = m_audioSource.volume;

        if (m_startingMusic != EMessageType.none)
        {
            PlayMusic(m_startingMusic.ToString());
        }
    }

    public void PlayMusic(string _name)
    {
        StopAllCoroutines();
        StartCoroutine(SwitchMusic(_name));

        //if (m_audioSource.isPlaying)
        //{
        //    // If the clip playing is the one wanted to be played, return
        //    if (m_audioSource.clip.name == _name)
        //    {
        //        return; 
        //    }

        //    // If the clip playing is not the one wanted, stop it playing
        //    m_audioSource.Stop();
        //}

        //m_audioSource.clip = m_musicDictionary[_name];

        //m_audioSource.volume = m_defaultVolume;

        //m_audioSource.Play();
    }

    private IEnumerator SwitchMusic(string _name)
    {
        if (m_audioSource.isPlaying)
        {
            // If the clip playing is the one wanted to be played, return
            if (m_audioSource.clip.name == _name)
            {
                yield break;
            }

            // If the clip playing is not the one wanted, stop it playing
            
            m_audioSource.DOFade(0.0f, 1.0f);
            yield return new WaitForSeconds(1.0f);
            m_audioSource.Stop();
        }

        m_audioSource.volume = 0.0f;

        m_audioSource.clip = m_musicDictionary[_name];

        m_audioSource.volume = m_defaultVolume;

        m_audioSource.Play();

        m_audioSource.DOFade(1.0f, 1.0f);
    }

    private void StopMusic(string _null)
    {
        m_audioSource.Stop();
    }
    
    // Fades the music in/out over a duration
    public IEnumerator FadeMusic(float _duration, bool _fadeOut = true)
    {
        float timer = 0.0f;
        float startVolume = m_audioSource.volume;

        if (!_fadeOut)
        {
            m_audioSource.Play();
        }
        while (timer < _duration)
        {
            timer += Time.deltaTime;
            float endResult = (_fadeOut) ? 0.0f : 1.0f;
            m_audioSource.volume = Mathf.Lerp(startVolume, endResult, timer / _duration);
            yield return null;
        }

        if (_fadeOut)
        {
            m_audioSource.volume = 0.0f;
            m_audioSource.Pause();
        }
        else
        {
            m_audioSource.volume = 1.0f;
        }
    }

    public void AEFadeMusicOut(float _duration)
    {
        StartCoroutine(FadeMusic(_duration, true));
    }
}
