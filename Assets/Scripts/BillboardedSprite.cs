using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardedSprite : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
    }
}
