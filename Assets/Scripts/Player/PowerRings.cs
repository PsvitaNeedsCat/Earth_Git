using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerRings : MonoBehaviour
{
    [SerializeField] private GameObject[] m_ringObjects = new GameObject[3];
    private Animator[] m_ringAnimators = new Animator[3];

    // Gets the animators of the rings and stores them
    private void Awake()
    {
        for (int i = 0; i < m_ringObjects.Length; i++)
        {
            m_ringAnimators[i] = m_ringObjects[i].GetComponent<Animator>();
        }
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
        for (int i = 0; i < m_ringObjects.Length; i++)
        {
            if (i == (int)_power)
            {
                m_ringObjects[i].SetActive(true);
                m_ringAnimators[i].Play("Ring_Expand");
                continue;
            }

            m_ringObjects[i].SetActive(false);
            m_ringAnimators[i].StopPlayback();
        }
    }
}
