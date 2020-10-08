using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenshakeManager : MonoBehaviour
{
    public enum EShakeType 
    {
        small,
        medium,
        shortSharp, 
        centipedeHitChunk,
    }

    [SerializeField] private CinemachineImpulseSource m_smallShake;
    [SerializeField] private CinemachineImpulseSource m_mediumShake;
    [SerializeField] private CinemachineImpulseSource m_shortSharpShake;
    [SerializeField] private CinemachineImpulseSource m_centipedeChunkHitShake;

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
            case EShakeType.centipedeHitChunk: s_instance.m_centipedeChunkHitShake.GenerateImpulse();
                break;
        }
    }
}
