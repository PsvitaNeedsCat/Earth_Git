using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLaser : MonoBehaviour
{
    public BoxCollider m_leftLaser;
    public BoxCollider m_rightLaser;

    private bool m_isFiring = false;

    public void FireLaserFor(float _duration)
    {
        if (m_isFiring) return;

        // UpdateSettings();
        UpdateLaser(m_leftLaser, Vector3.left);
        UpdateLaser(m_rightLaser, Vector3.right);

        StartFiring();

        StartCoroutine(StopFiringAfter(_duration));
    }

    private void Update()
    {
        // if (m_isFiring) UpdateLasers();
        if (m_isFiring)
        {
            UpdateLaser(m_leftLaser, Vector3.left);
            UpdateLaser(m_rightLaser, Vector3.right);
        }
    }

    //public void UpdateSettings()
    //{
    //    CentipedeStateInfo currentStateInfo = CentipedeBoss.GetCurrentStateInfo();
    //    Vector3 newSize = new Vector3(currentStateInfo.m_laserDistance, 1.0f, currentStateInfo.m_laserWidth);
    //    m_leftLaser.center = Vector3.left * (0.5f + currentStateInfo.m_laserDistance * 0.5f);
    //    m_rightLaser.center = Vector3.right * (0.5f + currentStateInfo.m_laserDistance * 0.5f);

    //    m_leftLaser.size = newSize;
    //    m_rightLaser.size = newSize;
    //}

    private void UpdateLaser(BoxCollider _laser, Vector3 _direction)
    {
        Vector3 laserWorldPos = transform.position + (transform.rotation * _direction * 0.5f);
        CentipedeStateInfo currentStateInfo = CentipedeBoss.GetCurrentStateInfo();
        RaycastHit hitInfo;
        Ray ray = new Ray(laserWorldPos, (transform.rotation * _direction));

        float endDist;
        Vector3 endSize;

        // If hit, scale collider
        if (Physics.Raycast(ray, out hitInfo, currentStateInfo.m_laserDistance, CentipedeBoss.m_settings.m_blocksLasers))
        {
            float dist = (hitInfo.point - laserWorldPos).magnitude;
            endSize = new Vector3(dist, _laser.size.y, _laser.size.z);
            endDist = dist;
        }
        // Otherwise, go to default distance
        else
        {
            endSize = new Vector3(currentStateInfo.m_laserDistance, 1.0f, currentStateInfo.m_laserWidth);
            endDist = currentStateInfo.m_laserDistance;
        }

        // Move and scale collider
        _laser.size = endSize;
        _laser.center = _direction * (0.5f + endDist * 0.5f);
    }

    //private void UpdateLasers()
    //{
    //    CentipedeStateInfo currentStateInfo = CentipedeBoss.GetCurrentStateInfo();

    //    RaycastHit hitInfo;
    //    Ray ray = new Ray(m_leftLaser.transform.position, Vector3.left);

    //    if (Physics.Raycast(ray, out hitInfo, currentStateInfo.m_laserDistance, CentipedeBoss.m_settings.m_blocksLasers))
    //    {
    //        float dist = (hitInfo.point - m_leftLaser.transform.position).magnitude;
    //        Vector3 newSize = new Vector3(dist, m_leftLaser.size.y, m_leftLaser.size.z);
    //        m_leftLaser.size = newSize;
    //        m_leftLaser.center = Vector3.left * (0.5f + dist * 0.5f);
    //    }

    //    ray.origin = m_rightLaser.transform.position;
    //    ray.direction = Vector3.right;

    //    if (Physics.Raycast(ray, out hitInfo, currentStateInfo.m_laserDistance, CentipedeBoss.m_settings.m_blocksLasers))
    //    {
    //        float dist = (hitInfo.point - m_rightLaser.transform.position).magnitude;
    //        Vector3 newSize = new Vector3(dist, m_rightLaser.size.y, m_rightLaser.size.z);
    //        m_rightLaser.size = newSize;
    //        m_rightLaser.center = Vector3.right * (0.5f + dist * 0.5f);
    //    }
    //}

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
    }

    private void StopFiring()
    {
        m_isFiring = false;

        m_leftLaser.enabled = false;
        m_rightLaser.enabled = false;
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
