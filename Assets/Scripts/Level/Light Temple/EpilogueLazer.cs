using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class EpilogueLazer : MonoBehaviour
{
    [SerializeField] private UnityEvent m_statueHitEvent = new UnityEvent();

    private int m_punchedCount = 4; // Check after punch
    private AudioSource m_laserSound = null;

    private void Awake()
    {
        m_laserSound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        m_laserSound.Play();
    }

    // Called by EpilogueChunk - counts down the punches until it reaches the statue
    public void ChunkPunched()
    {
        --m_punchedCount;

        if (m_punchedCount <= 0)
        {
            m_laserSound.Stop();
            m_statueHitEvent.Invoke();
            Destroy(gameObject);
        }
        else
        {
            // Move forward (toward statue)
            Vector3 newPos = transform.position;
            newPos.z += 1.0f;
            transform.position = newPos;
        }
    }
}
