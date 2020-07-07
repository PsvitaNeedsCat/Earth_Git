﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntry : Interactable
{
    [SerializeField] private string m_levelName;
    [SerializeField] private int m_id;

    private RoomManager m_roomManager = null;
    private Overworld m_overworldManager = null;

    public override void Awake()
    {
        base.Awake();

        m_roomManager = FindObjectOfType<RoomManager>();
        m_overworldManager = FindObjectOfType<Overworld>();
    }

    // Load level
    public override void Invoke()
    {
        base.Invoke();

        if (!m_overworldManager.IsLevelUnlocked(m_id)) { return; }

        m_roomManager.LoadScene(m_levelName);
    }
}
