using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard_SCR : MonoBehaviour
{
    [SerializeField] Transform cam = null;
    Quaternion startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.position, Vector3.up);

        transform.rotation = new Quaternion(startPos.x, transform.rotation.y, startPos.z, startPos.w);
    }
}
