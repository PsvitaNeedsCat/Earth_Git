using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedStars : MonoBehaviour
{
    [SerializeField] private GameObject[] m_stars = new GameObject[3];
    private float m_stunnedTimer = 0.0f;
    private float m_stunnedTimerInit = 0.0f;
    private bool m_areStarsActive = false;
    
    // Checks how low the stunned timer is and deactivates them as the timer ticks down every third
    private void FixedUpdate()
    {
        if (!m_areStarsActive)
        {
            return;
        }

        if (m_stunnedTimer <= 0.0f)
        {
            TurnStarsOff();
            return;
        }

        if (m_stars[0].activeSelf && m_stunnedTimer <= m_stunnedTimerInit * 0.66f)
        {
            m_stars[0].SetActive(false);
        }
        if (m_stars[1].activeSelf && m_stunnedTimer <= m_stunnedTimerInit * 0.33f)
        {
            m_stars[1].SetActive(false);
        }

        m_stunnedTimer -= Time.fixedDeltaTime;
    }

    // Called when the enemy attached is stunned. Sets the time that enemy will be stunned for and resets the stars
    public void Init(float _stunnedTime)
    {
        m_stunnedTimer = _stunnedTime;
        m_stunnedTimerInit = _stunnedTime;

        float startTime = 0.0f;
        foreach (GameObject star in m_stars)
        {
            star.SetActive(true);
            ParticleSystem ps = star.GetComponent<ParticleSystem>();
            ps.Simulate(startTime);
            ps.Play();
            startTime += 2.0f;
        }

        m_areStarsActive = true;
    }

    // Stops all the particles and turns the stunned timer off
    private void TurnStarsOff()
    {
        m_areStarsActive = false;

        foreach (GameObject star in m_stars)
        {
            star.SetActive(false);
        }
    }

    // Stops the particles - can be called from external source
    public void ForceStop()
    {
        TurnStarsOff();
    }
}
