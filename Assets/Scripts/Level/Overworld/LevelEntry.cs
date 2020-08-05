using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntry : Interactable
{
    [SerializeField] private string m_levelName;
    [SerializeField] private int m_id;

    [SerializeField] private GameObject m_lockedIcon;

    private RoomManager m_roomManager = null;
    private Overworld m_overworldManager = null;
    
    private bool m_isLocked;

    public override void Awake()
    {
        base.Awake();

        m_roomManager = FindObjectOfType<RoomManager>();
        m_overworldManager = FindObjectOfType<Overworld>();

        m_isLocked = !m_overworldManager.IsLevelUnlocked(m_id);

        if (m_isLocked) { m_lockedIcon.SetActive(true); }
    }

    // Load level
    public override void Invoke()
    {
        base.Invoke();

        if (m_isLocked) { return; }

        m_roomManager.LoadScene(m_levelName);
    }

    public override void Update()
    {
        // Don't check if locked
        if (m_isLocked) { return; }

        base.Update();
    }
}
