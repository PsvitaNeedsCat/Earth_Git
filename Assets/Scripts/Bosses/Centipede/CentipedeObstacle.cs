using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeObstacle : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
