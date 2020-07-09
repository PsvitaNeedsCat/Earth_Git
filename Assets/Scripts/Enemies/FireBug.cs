using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class FireBug : MonoBehaviour
{
    public enum EFireBugState
    {
        none,
        patrolling,
        charging,
        vulnerable
    }

    public List<Transform> m_patrolPoints;
    public float m_moveSpeed;
    public float m_chargeSpeed;
    public float m_arriveDistance;
    public float m_windUpTime;
    public float m_vulnerableTime;
    public LayerMask m_visionHitLayers;
    public LayerMask m_chargeHitLayers;

    private int m_currentPointIndex = 0;
    [SerializeField] private EFireBugState m_state = EFireBugState.patrolling;
    private Vector3 m_chargeTarget;
    private Vector3 m_chargeDir;
    private Rigidbody m_rigidBody;

    private void Awake()
    {
        transform.LookAt(m_patrolPoints[m_currentPointIndex]);
        m_rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (m_state)
        {
            case EFireBugState.patrolling:
                {
                    Patrol();
                    break;
                }

            case EFireBugState.charging:
                {
                    Charge();
                    break;
                }

            case EFireBugState.vulnerable:
                {
                    Vulnerable();
                    break;
                }
        }
    }

    private void Patrol()
    {
        CheckPatrolPoint();

        Transform targetPoint = m_patrolPoints[m_currentPointIndex];
        transform.LookAt(m_patrolPoints[m_currentPointIndex]);

        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPoint.position, Time.deltaTime * m_moveSpeed);
        m_rigidBody.MovePosition(newPos);

        CheckForPlayer();
    }

    private void Charge()
    {
        transform.LookAt(m_chargeTarget);

        Vector3 newPos = Vector3.MoveTowards(transform.position, m_chargeTarget, Time.deltaTime * m_chargeSpeed);
        m_rigidBody.MovePosition(newPos);
    }
    
    private void Vulnerable()
    {

    }

    private void CheckPatrolPoint()
    {
        float dist = (transform.position - m_patrolPoints[m_currentPointIndex].position).magnitude;

        if (dist < m_arriveDistance)
        {
            m_currentPointIndex = (m_currentPointIndex + 1) % m_patrolPoints.Count;
            transform.LookAt(m_patrolPoints[m_currentPointIndex]);
        }
    }    

    private void CheckForPlayer()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, transform.forward, out hitInfo, 100.0f, m_visionHitLayers))
        {
            if (hitInfo.collider.gameObject.GetComponent<Player>())
            {
                Debug.Log("Found player!");
                m_chargeDir = transform.forward;
                StartCoroutine(BeginCharge());
            }
        }
    }

    private IEnumerator BeginCharge()
    {
        m_state = EFireBugState.none;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, m_chargeDir, out hitInfo, 100.0f, m_chargeHitLayers))
        {
            m_chargeTarget = hitInfo.point - Vector3.up * 0.2f;
            Debug.Log("Hit " + hitInfo.collider.name);
        }
        else
        {
            Debug.Log("Didn't find a target");
            m_chargeTarget = transform.position + Vector3.up * 0.2f + m_chargeDir * 100.0f;
        }

        transform.DOBlendableLocalRotateBy(Vector3.forward * 90.0f, m_windUpTime, RotateMode.LocalAxisAdd);

        yield return new WaitForSeconds(m_windUpTime);

        transform.DOBlendableLocalRotateBy(Vector3.forward * -90.0f, m_windUpTime / 10.0f, RotateMode.LocalAxisAdd);

        yield return new WaitForSeconds(m_windUpTime / 10.0f);

        m_state = EFireBugState.charging;
    }

    private IEnumerator FlipOver()
    {
        m_state = EFireBugState.vulnerable;

        transform.DORotate(Vector3.forward * 180.0f, 0.5f);

        yield return new WaitForSeconds(m_vulnerableTime);

        transform.DORotate(Vector3.zero, 0.5f);

        m_state = EFireBugState.patrolling;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_state == EFireBugState.charging)
        {
            StartCoroutine(FlipOver());
        }
    }

    private void OnDrawGizmos()
    {
        if (m_chargeTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_chargeTarget, 1.0f);
            Gizmos.DrawLine(transform.position + Vector3.up * 0.2f, transform.position + Vector3.up * 0.2f + m_chargeDir * 100.0f);
        }
    }
}
