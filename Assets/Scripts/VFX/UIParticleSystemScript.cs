using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParticleSystemScript : MonoBehaviour
{
    [Header("Particle")]
    [SerializeField] private float Duration = 5.0f;
    [SerializeField] private bool Looping = true;
    [SerializeField] private float startLifetime = 1.0f;
    [SerializeField] private float startSpeed = 1.0f;
    [SerializeField] private Vector3 startSize = new Vector3(1, 1, 1);
    [SerializeField] private float startRotation = 0.0f;
    [SerializeField] private float gravity = 0.0f;

    [Space]
    [Header("Emission")]
    [SerializeField] private float RateOverTime = 1.0f;
    private float currentRate = 0.0f;

    [Space]
    [Header("Shape")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX, minY, maxY;

    [Space]

    [Header("Colour Over Lifetime")]
    //[SerializeField] private bool changeColour = false;
    [SerializeField] private Gradient Colour;

    [Space]
    [Header("Size Over Lifetime")]
    //[SerializeField] private bool changeSpeed = false;
    [SerializeField] private AnimationCurve Size;

    [Space]
    [Header("Rotation Over Lifetime")]
    //[SerializeField] private bool changeRotation = false;
    [SerializeField] private float minRotation, maxRotation;

    [Space]
    [Header("Renderer")]
    [SerializeField] private GameObject particleBase;
    [SerializeField] private Material material;


    void Update()
    {
        if (currentRate <= 0.0f)
        {
            CreateParticle();

            currentRate = 1.0f / RateOverTime;
        }
        else
        {
            currentRate -= Time.deltaTime;
        }

        if (!Looping)
        {
            Duration -= Time.deltaTime;

            if (Duration <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void CreateParticle()
    {
        GameObject newParticle = Instantiate(particleBase, transform.position, transform.rotation) as GameObject;

        newParticle.transform.SetParent(transform);

        UIParticleScript ps = newParticle.GetComponent<UIParticleScript>();

        ps.startLifetime = startLifetime;
        ps.startSpeed = startSpeed;
        ps.startSize = startSize;
        ps.startRotation = startRotation;
        ps.gravity = gravity;
        ps.Colour = Colour;
        ps.Size = Size;
        ps.minRotation = minRotation;
        ps.maxRotation = maxRotation;
        ps.mat = material;
    }
}
