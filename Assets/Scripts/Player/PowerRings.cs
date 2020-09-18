using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerRings : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] m_ringParticles = new ParticleSystem[3];

    private void Start()
    {
        SetPower(Player.s_currentEffect);
    }

    // Adds and removes event listeners for the player changing powers
    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.powerRock, SetRockPower);
        MessageBus.AddListener(EMessageType.powerWater, SetWaterPower);
        MessageBus.AddListener(EMessageType.powerFire, SetFirePower);
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.powerRock, SetRockPower);
        MessageBus.RemoveListener(EMessageType.powerWater, SetWaterPower);
        MessageBus.RemoveListener(EMessageType.powerFire, SetFirePower);
    }

    // Calls teh SetPower function - updates rings' colour and animation
    private void SetRockPower(string _null)
    {
        SetPower(EChunkEffect.none);
    }
    private void SetWaterPower(string _null)
    {
        SetPower(EChunkEffect.water);
    }
    private void SetFirePower(string _null)
    {
        SetPower(EChunkEffect.fire);
    }

    // Updates the rings so that the new power does an animation then stays visible
    private void SetPower(EChunkEffect _power)
    {
        for (int i = 0; i < m_ringParticles.Length; i++)
        {
            if (i == (int)_power)
            {
                m_ringParticles[i].Play();
                continue;
            }

            m_ringParticles[i].Stop();
        }
    }
}
