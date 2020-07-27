using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

using DG.Tweening;

public class FireBug : MonoBehaviour
{
    public enum EFireBugState
    {
        none,
        patrolling,
        charging,
        vulnerable,
        waiting,
        returning
    }

    [SerializeField] private List<Transform> m_patrolPoints;
    public LayerMask m_visionHitLayers;
    public LayerMask m_chargeHitLayers;
    public GameObject m_mesh;

    private float m_moveSpeed = 1.5f;
    private float m_chargeSpeed = 6.0f;
    private float m_arriveDistance = 0.2f;
    private float m_windUpTime = 1.0f;
    private float m_vulnerableTime = 5.0f;
    private float m_turnTime = 0.6f;
    private int m_currentPatrolPointIndex = 0;
    private EFireBugState m_state = EFireBugState.patrolling;
    private Vector3 m_chargeTarget;
    private Vector3 m_chargeDir;
    private Rigidbody m_rigidBody;
    private Quaternion m_startRotation;
    [SerializeField] private bool m_shouldPatrol = true;

    private void Awake()
    {
        transform.LookAt(m_patrolPoints[m_currentPatrolPointIndex]);
        m_rigidBody = GetComponent<Rigidbody>();
        m_startRotation = transform.rotation;
    }

    private void OnEnable()
    {
        m_state = EFireBugState.patrolling;
        m_currentPatrolPointIndex = 0;
        m_mesh.transform.rotation = m_startRotation;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void FixedUpdate()
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

            case EFireBugState.waiting:
                {
                    CheckForPlayer();
                    break;
                }

            case EFireBugState.returning:
                {
                    ReturnToInitPoint();
                    break;
                }
        }
    }

    private void ReturnToInitPoint()
    {
        m_currentPatrolPointIndex = 0;

        // Move towards point
        MoveTowardsCurrentPoint();

        CheckForPlayer();

        // If reached point, then stop
        CheckPatrolPoint();
    }

    // Patrols between points
    private void Patrol()
    {
        if (!m_shouldPatrol)
        {
            m_state = EFireBugState.waiting;
        }

        CheckPatrolPoint();

        // Find current target point, and move towards it
        MoveTowardsCurrentPoint();

        CheckForPlayer();
    }

    private void MoveTowardsCurrentPoint()
    {
        Transform targetPoint = m_patrolPoints[m_currentPatrolPointIndex];
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPoint.position, Time.fixedDeltaTime * m_moveSpeed);
        m_rigidBody.MovePosition(newPos);
    }

    private void Charge()
    {
        // Update charge, keep looking at target and moving towards it
        // transform.LookAt(m_chargeTarget);
        Vector3 newPos = Vector3.MoveTowards(transform.position, m_chargeTarget, Time.fixedDeltaTime * m_chargeSpeed);
        m_rigidBody.MovePosition(newPos);
    }
    
    private void Vulnerable()
    {

    }

    private void CheckPatrolPoint()
    {
        float dist = (transform.position - m_patrolPoints[m_currentPatrolPointIndex].position).magnitude;

        if (dist < m_arriveDistance)
        {
            StartCoroutine(Turn());
            // transform.LookAt(m_patrolPoints[m_currentPointIndex]);
        }
    }    

    private void CheckForPlayer()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 100.0f, m_visionHitLayers))
        {
            if (hitInfo.collider.gameObject.GetComponent<Player>())
            {
                m_chargeDir = transform.forward;
                StartCoroutine(BeginCharge());
            }
        }
    }

    private IEnumerator BeginCharge()
    {
        m_state = EFireBugState.none;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, m_chargeDir, out hitInfo, 100.0f, m_chargeHitLayers, QueryTriggerInteraction.Ignore))
        {
            m_chargeTarget = hitInfo.point;
        }
        else
        {
            m_chargeTarget = transform.position + m_chargeDir * 100.0f;
        }

        // m_mesh.transform.DOBlendableLocalRotateBy(m_mesh.transform.right * 90.0f, m_windUpTime, RotateMode.LocalAxisAdd);

        yield return new WaitForSeconds(m_windUpTime);

        // m_mesh.transform.DOBlendableLocalRotateBy(m_mesh.transform.right * -90.0f, m_windUpTime / 10.0f, RotateMode.LocalAxisAdd);

        yield return new WaitForSeconds(m_windUpTime / 10.0f);

        m_state = EFireBugState.charging;
    }

    private IEnumerator FlipOver()
    {
        m_state = EFireBugState.vulnerable;

        m_mesh.transform.DORotate(Vector3.forward * 180.0f, 0.5f);

        yield return new WaitForSeconds(m_vulnerableTime);

        // m_mesh.transform.DORotate(Vector3.right, 0.5f);
        m_mesh.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.5f).OnComplete(() => transform.DOLookAt(m_patrolPoints[m_currentPatrolPointIndex].position, 0.2f));

        m_state = (m_shouldPatrol) ? EFireBugState.patrolling : EFireBugState.returning;
    }

    private IEnumerator Turn()
    {
        m_currentPatrolPointIndex = (m_currentPatrolPointIndex + 1) % m_patrolPoints.Count;
        m_state = EFireBugState.none;

        // m_mesh.transform.DOBlendableLocalRotateBy(Vector3.up * 180.0f, m_turnTime, RotateMode.LocalAxisAdd);
        transform.DOLookAt(m_patrolPoints[m_currentPatrolPointIndex].position, m_turnTime);
        yield return new WaitForSeconds(m_turnTime);

        m_state = EFireBugState.patrolling;
    }

    public void Hit(EChunkEffect _effect)
    {
        if (_effect == EChunkEffect.none && m_state != EFireBugState.vulnerable)
        {
            StopAllCoroutines();
            StartCoroutine(FlipOver());
            return;
        }
        
        if (_effect == EChunkEffect.water && m_state == EFireBugState.vulnerable)
        {
            MessageBus.TriggerEvent(EMessageType.fireBugKilled);
            Destroy(this.gameObject);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        Player player = other.GetComponent<Player>();

        if (player && m_state == EFireBugState.charging)
        {
            HealthComponent healthComp = player.GetComponent<HealthComponent>();
            healthComp.Health -= 1;
            return;
        }
        else if (player && m_state != EFireBugState.charging)
        {
            return;
        }

        if (m_state == EFireBugState.charging)
        {
            StopAllCoroutines();
            StartCoroutine(FlipOver());
            return;
        }

        if (m_state == EFireBugState.patrolling && !other.isTrigger)
        {
            StopAllCoroutines();
            StartCoroutine(Turn());
            return;
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine(transform.position + Vector3.up * 0.2f, transform.position + Vector3.up * 0.2f + transform.forward * 100.0f);

        if (m_chargeTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_chargeTarget, 1.0f);
            Gizmos.DrawLine(transform.position, transform.position + m_chargeDir * 100.0f);
        }
    }
}
