using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenshakeManager : MonoBehaviour
{
    public enum EShakeType { small, medium, shortSharp, }

    public CinemachineImpulseSource m_smallShake;
    public CinemachineImpulseSource m_mediumShake;
    public CinemachineImpulseSource m_shortSharpShake;

    private static ScreenshakeManager s_instance;

    private void Awake()
    {
        if (s_instance && s_instance != this)
        {
            Destroy(s_instance.gameObject);
        }

        s_instance = this;
    }

    public static void Shake(EShakeType _type)
    {
        switch (_type)
        {
            case EShakeType.small: s_instance.m_smallShake.GenerateImpulse();
                break;
            case EShakeType.medium: s_instance.m_mediumShake.GenerateImpulse();
                break;
            case EShakeType.shortSharp: s_instance.m_shortSharpShake.GenerateImpulse();
                break;
        }
    }
}
