using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCube : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 pos = transform.position;
        pos.y += 10.0f;
        transform.position = pos;
    }
}
