using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow_SCR : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask = 0;
    private float yOffset = 0.1f;

    // Update is called once per frame
    void Update()
    {
        Ray ray;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundMask))
        {
            Vector3 newPos = new Vector3(hit.point.x, hit.point.y + yOffset, hit.point.z);

            transform.position = newPos;
        }
    }
}
