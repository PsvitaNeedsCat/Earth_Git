using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParticleScript : MonoBehaviour
{
    private Image rend;
    private Rigidbody2D rb;

    [Header("Particle")]
    public float startLifetime = 1.0f;
    public float startSpeed = 1.0f;
    public Vector3 startSize = new Vector3(1, 1, 1);
    public float startRotation = 0.0f;
    public float gravity = 0.0f;

    [Space]

    [Header("Colour Over Lifetime")]
    public Gradient Colour;

    [Space]
    [Header("Size Over Lifetime")]
    public AnimationCurve Size;

    [Space]
    [Header("Rotation Over Lifetime")]
    public float minRotation;
    public float maxRotation;
    private float myRot;

    [Space]
    [Header("Renderer")]
    public Material mat;


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Image>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = gravity * 2;

        rb.AddForce(transform.up * (startSpeed * 10), ForceMode2D.Impulse);
        rend.material = mat; 

        //Colour
        rend.color = Colour.Evaluate(0.0f);

        //Scale
        transform.localScale = startSize;

        //Rotation
        transform.rotation = Quaternion.Euler(0, 0, startRotation);
        myRot = Random.Range(minRotation, maxRotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (startLifetime > 0)
        {
            startLifetime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }

        //Colour
        rend.color = Colour.Evaluate(1.0f / startLifetime);

        //Rotation
        if (myRot != 0)
        {
            transform.Rotate(0, 0, myRot * Time.deltaTime);
        }

        //Scale
        float myScale = Size.Evaluate(1.0f / startLifetime);
        transform.localScale = new Vector3(myScale, myScale, myScale);
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
