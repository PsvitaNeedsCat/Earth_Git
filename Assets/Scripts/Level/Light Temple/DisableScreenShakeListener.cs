using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

[RequireComponent(typeof(CinemachineImpulseListener))]
public class DisableScreenShakeListener : MonoBehaviour
{
    // Sets the gain to 0 so that there's no screen shake
    public void StopScreenShake()
    {
        GetComponent<CinemachineImpulseListener>().m_Gain = 0.0f;
    }
}
