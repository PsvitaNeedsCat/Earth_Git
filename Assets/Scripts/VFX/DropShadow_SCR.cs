using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow_SCR : MonoBehaviour
{
    [SerializeField] private LayerMask m_groundMask = 0;
    private float m_yOffset = 0.01f;
    private float m_initY = 0.0f;

    private void Awake()
    {
        m_initY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 centre = new Vector3(transform.position.x, m_initY, transform.position.z);

        if (Physics.Raycast(centre, Vector3.down, out hit, Mathf.Infinity, m_groundMask))
        {
            Vector3 newPos = new Vector3(hit.point.x, hit.point.y + m_yOffset, hit.point.z);

            transform.position = newPos;
        }
    }
}
