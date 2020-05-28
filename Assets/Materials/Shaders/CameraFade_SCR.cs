using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade_SCR : MonoBehaviour
{
    private Material mat;
    private float alpha = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Material>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
