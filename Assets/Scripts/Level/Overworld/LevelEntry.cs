using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntry : Interactable
{
    [SerializeField] private string m_levelName;
    [SerializeField] private int m_id;

    [SerializeField] private GameObject m_interactIcon;
    [SerializeField] private GameObject m_lockedIcon;

    private RoomManager m_roomManager = null;
    private Overworld m_overworldManager = null;

    private GlobalPlayerSettings m_playerSettings;
    private bool m_promptActive = false;
    private bool m_isLocked;

    public override void Awake()
    {
        base.Awake();

        m_roomManager = FindObjectOfType<RoomManager>();
        m_overworldManager = FindObjectOfType<Overworld>();
        m_playerSettings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");

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

    private void Update()
    {
        // Don't check if locked
        if (m_isLocked) { return; }

        bool playerIsCloseEnough = (m_playerRef.transform.position - transform.position).magnitude < m_playerSettings.m_maxInteractableDist;

        // Player is close enough to interact - and prompt is not currently active
        if (!m_promptActive && playerIsCloseEnough)
        {
            // Turn prompt on
            m_promptActive = true;
            m_interactIcon.SetActive(true);
        }
        // Player is not close enough to interact - and prompt is currently active
        else if (m_promptActive && !playerIsCloseEnough)
        {
            // Turn prompt off
            m_promptActive = false;
            m_interactIcon.SetActive(false);
        }
    }
}
