using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToHere : MonoBehaviour
{
    [SerializeField] private GameObject m_objectToTeleport = null;

    private void Start()
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
