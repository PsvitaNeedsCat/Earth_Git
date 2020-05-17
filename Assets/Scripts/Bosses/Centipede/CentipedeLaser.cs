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

        UpdateSettings();

        StartFiring();

        StartCoroutine(StopFiringAfter(_duration));
    }

    public void UpdateSettings()
    {
        CentipedeStateInfo currentStateInfo = CentipedeBoss.GetCurrentStateInfo();
        Vector3 newSize = new Vector3(currentStateInfo.m_laserDistance, 1.0f, currentStateInfo.m_laserWidth);
        m_leftLaser.center = (Vector3.left * (0.5f + currentStateInfo.m_laserDistance * 0.5f));
        m_rightLaser.center = (Vector3.right * (0.5f + currentStateInfo.m_laserDistance * 0.5f));

        m_leftLaser.size = newSize;
        m_rightLaser.size = newSize;
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
}
