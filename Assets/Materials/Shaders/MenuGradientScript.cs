using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGradientScript : MonoBehaviour
{
    private MeshRenderer mr;

    public Gradient gradient1;
    public Gradient gradient2;

    public float speedMultipler = 0.5f;
    private float gradientTime = 0.0f;

    private Color c1;
    private Color c2;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        gradientTime = Mathf.PingPong(Time.time * speedMultipler, 1);

        c1 = (gradient1.Evaluate(gradientTime));
        mr.material.SetColor("_Color", c1);

        c2 = (gradient2.Evaluate(gradientTime));
        mr.material.SetColor("_Color1", c2);
    }
}
