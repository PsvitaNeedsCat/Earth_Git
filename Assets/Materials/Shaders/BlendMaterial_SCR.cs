using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendMaterial_SCR : MonoBehaviour
{
    Renderer rend;

    private float blendVal = 0.0f;
    [SerializeField] private float speed;

    [SerializeField] private string materialName = "";

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Earth/" + materialName);
    }

    // Update is called once per frame
    void Update()
    {
        blendVal = Mathf.Sin(Time.fixedTime * Mathf.PI * speed) / 2.0f + 0.5f;

        rend.material.SetFloat("_Blend", blendVal);
    }
}