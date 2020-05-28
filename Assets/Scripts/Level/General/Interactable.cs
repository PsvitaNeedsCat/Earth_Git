using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [HideInInspector] public static Interactable m_closest = null; // Closest to the player
    [HideInInspector] public float m_distToPlayer = float.MaxValue;

    private static Player m_playerRef;

    private void Awake()
    {
        // The first one gets the player reference
        if (!m_playerRef)
        {
            Player player = FindObjectOfType<Player>();
            if (player) { m_playerRef = player; }
        }
    }

    // Message bus listener
    private void OnEnable() => MessageBus.AddListener(EMessageType.interact, CheckForClosest);
    private void OnDisable() => MessageBus.RemoveListener(EMessageType.interact, CheckForClosest);

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
}
