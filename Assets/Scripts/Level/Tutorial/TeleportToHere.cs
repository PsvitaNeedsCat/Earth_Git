using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToHere : MonoBehaviour
{
    [SerializeField] private GameObject m_objectToTeleport = null;
    [SerializeField] private bool m_invokeOnStart = true;

    private void Start()
    {
        if (m_invokeOnStart)
        {
            Teleport();
        }
    }

    public void Teleport()
    {
        if (!m_objectToTeleport)
        {
            return;
        }

        m_objectToTeleport.transform.position = transform.position;
        m_objectToTeleport.transform.rotation = transform.rotation;

        Destroy(gameObject);
    }
}
