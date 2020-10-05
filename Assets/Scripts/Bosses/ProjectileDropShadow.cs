using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDropShadow : MonoBehaviour
{
    public float m_dropShadowMaxRange;
    public LayerMask m_dropShadowLayerMask;
    public GameObject m_dropShadow;

    public float m_dropShadowMinScale;
    public float m_dropShadowMaxScale;

    private bool m_enabled = true;

    public void SetDropShadow(bool _on)
    {
        m_enabled = _on;
    }

    private void Update()
    {
        UpdateDropShadow();
    }

    private void UpdateDropShadow()
    {
        RaycastHit hitInfo;

        if (!m_enabled)
        {
            m_dropShadow.SetActive(false);
            return;
        }

        // If there is a surface below
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_dropShadowMaxRange, m_dropShadowLayerMask))
        {
            // If surface is out of range, deactivate drop shadow
            if (hitInfo.distance > m_dropShadowMaxRange || hitInfo.distance < 0.0f)
            {
                m_dropShadow.SetActive(false);
                return;
            }

            // Otherwise, scale drop shadow based on distance, and place it just above floor
            float newScale = Mathf.Lerp(m_dropShadowMinScale, m_dropShadowMaxScale, (m_dropShadowMaxRange - hitInfo.distance) / m_dropShadowMaxRange);
            m_dropShadow.SetActive(true);
            m_dropShadow.transform.localScale = Vector3.one * newScale;
            m_dropShadow.transform.position = hitInfo.point + Vector3.up * 0.01f;
        }
        else
        {
            m_dropShadow.SetActive(false);
        }
    }
}
