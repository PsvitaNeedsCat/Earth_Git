using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntry : Interactable
{
    [SerializeField] private string m_levelName;
    [SerializeField] private int m_id;

    [SerializeField] private GameObject m_completedIcon = null;

    private RoomManager m_roomManager = null;
    private Overworld m_overworldManager = null;

    public override void Awake()
    {
        base.Awake();

        m_roomManager = FindObjectOfType<RoomManager>();
        m_overworldManager = FindObjectOfType<Overworld>();

        if (Player.s_activePowers[(EChunkEffect)m_id + 1])
        {
            m_completedIcon.SetActive(true);
        }
    }

    // Load level
    public override void Invoke()
    {
        m_roomManager.LoadScene(m_levelName);
    }
}
