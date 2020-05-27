using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLaser : MonoBehaviour
{
    public BoxCollider m_leftLaser;
    public BoxCollider m_rightLaser;
    public LineRenderer m_leftLine;
    public LineRenderer m_rightLine;

    private bool m_isFiring = false;

    public void FireLaserFor(float _duration)
    {
        if (m_isFiring) return;

        float lineWidth = CentipedeBoss.m_settings.m_laserWidth;
        m_leftLine.startWidth = lineWidth;
        m_leftLine.endWidth = lineWidth;
        m_rightLine.startWidth = lineWidth;
        m_rightLine.endWidth = lineWidth;

        UpdateLaser(m_leftLaser, m_leftLine, Vector3.left);
        UpdateLaser(m_rightLaser, m_rightLine, Vector3.right);

        StartFiring();

        StartCoroutine(StopFiringAfter(_duration));
    }

    private void Update()
    {
        if (m_isFiring)
        {
            UpdateLaser(m_leftLaser, m_leftLine, Vector3.left);
            UpdateLaser(m_rightLaser, m_rightLine, Vector3.right);
        }
    }

    private void UpdateLaser(BoxCollider _laser, LineRenderer _line, Vector3 _direction)
    {
        Vector3 laserWorldPos = transform.position + (transform.rotation * _direction * 0.5f);
        
        RaycastHit hitInfo;
        Ray ray = new Ray(laserWorldPos, (transform.rotation * _direction));

        float endDist;
        Vector3 endSize;

        // If hit, scale collider
        if (Physics.Raycast(ray, out hitInfo, CentipedeBoss.m_settings.m_laserDistance, CentipedeBoss.m_settings.m_blocksLasers))
        {
            float dist = (hitInfo.point - laserWorldPos).magnitude;
            endSize = new Vector3(dist, _laser.size.y, _laser.size.z);
            endDist = dist;
        }
        // Otherwise, go to default distance
        else
        {
            endSize = new Vector3(CentipedeBoss.m_settings.m_laserDistance, 1.0f, CentipedeBoss.m_settings.m_laserWidth);
            endDist = CentipedeBoss.m_settings.m_laserDistance;
        }

        Vector3 endPos = _direction * (endDist + 0.5f);
        _line.SetPosition(1, endPos);

        // Move and scale collider
        _laser.size = endSize;
        _laser.center = _direction * (0.5f + endDist * 0.5f);
    }

    private IEnumerator StopFiringAfter(float _seconds)
    {
        yield return new WaitForSeconds(_seconds);

        StopFiring();
    }

    private void StartFiring()
    {
        m_isFiring = true;

        m_leftLaser.enabled = true;
        m_rightLaser.enabled = true;
        m_leftLine.enabled = true;
        m_rightLine.enabled = true;
    }

    private void StopFiring()
    {
        m_isFiring = false;

        m_leftLaser.enabled = false;
        m_rightLaser.enabled = false;
        m_leftLine.enabled = false;
        m_rightLine.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthComponent healthComp = other.GetComponent<HealthComponent>();

        if (healthComp && healthComp.m_type == HealthComponent.EHealthType.player)
        {
            healthComp.Health -= 1;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 leftLaserWorldPos = transform.position + 0.5f * (transform.rotation * Vector3.left);
        Vector3 rightLaserWorldPos = transform.position + 0.5f * (transform.rotation * Vector3.right);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftLaserWorldPos, 0.2f);
        Gizmos.DrawWireSphere(rightLaserWorldPos, 0.2f);
    }
}
