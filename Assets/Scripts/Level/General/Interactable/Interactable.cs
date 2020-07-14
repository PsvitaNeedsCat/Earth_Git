using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [HideInInspector] public static Interactable m_closest = null; // Closest to the player
    [HideInInspector] public float m_distToPlayer = float.MaxValue;

    private static Player m_playerRef;
    private bool m_playerIsClose = false;
    private GlobalPlayerSettings m_playerSettings;

    [SerializeField] protected GameObject m_prompt = null;

    public virtual void Awake()
    {
        // The first one gets the player reference
        if (!m_playerRef)
        {
            Player player = FindObjectOfType<Player>();
            if (player) { m_playerRef = player; }
        }

        m_playerSettings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
    }

    // Message bus listener
    public virtual void OnEnable() => MessageBus.AddListener(EMessageType.interact, CheckForClosest);
    public virtual void OnDisable() => MessageBus.RemoveListener(EMessageType.interact, CheckForClosest);

    virtual public void Invoke()
    {

    }

    // Compares distance to player with the current closest
    public void CheckForClosest(string _null)
    {
        // Set distance to the player
        m_distToPlayer = (m_playerRef.transform.position - transform.position).magnitude;

        // If there is nothing, this is the closest
        if (!m_closest)
        {
            m_closest = this;
            return;
        }

        // Compare distances to the player
        if (m_distToPlayer < m_closest.m_distToPlayer) { m_closest = this; }
    }

    public virtual void Update()
    {
        // Check if the player is close enough to trigger the prompt
        m_playerIsClose = (m_playerRef.transform.position - transform.position).magnitude < m_playerSettings.m_maxInteractableDist;

        // If there is a prompt
        if (m_prompt)
        {
            // Player is close enough to interact - prompt is not currently active
            if (!m_prompt.activeSelf && m_playerIsClose)
            {
                // Turn prompt on
                m_prompt.SetActive(true);
            }
            // Player is not close enough to interact - prompt is currently active
            else if (m_prompt.activeSelf && !m_playerIsClose)
            {
                // Turn off prompt
                m_prompt.SetActive(false);
            }
        }
    }
}
