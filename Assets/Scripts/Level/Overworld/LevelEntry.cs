using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntry : Interactable
{
    [SerializeField] private string m_levelName;
    [SerializeField] private int m_id;

    [SerializeField] private GameObject m_completedIcon = null;

    private RoomManager m_roomManager = null;

    public override void Awake()
    {
        base.Awake();

        m_roomManager = FindObjectOfType<RoomManager>();

        if (m_completedIcon && m_id + 1 < Player.s_activePowers.Count && Player.s_activePowers[(EChunkEffect)m_id + 1])
        {
            m_completedIcon.SetActive(true);
        }
    }

    // Load level
    public override void Invoke()
    {
        Player.m_lastTempleEntered = m_id + 1;

        m_roomManager.LoadScene(m_levelName);
    }
}
