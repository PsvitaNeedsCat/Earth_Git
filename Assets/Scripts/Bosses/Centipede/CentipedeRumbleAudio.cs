using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeRumbleAudio : MonoBehaviour
{
    public AudioSource m_rumbleSound;
    public Transform m_centipedeHead;
    public AnimationCurve m_soundDistanceCurve;

    private void Update()
    {
        // Update rumble sound volume based on distance to center
        float dist = (m_centipedeHead.position - transform.position).magnitude;
        m_rumbleSound.volume = m_soundDistanceCurve.Evaluate(dist);
    }
}
