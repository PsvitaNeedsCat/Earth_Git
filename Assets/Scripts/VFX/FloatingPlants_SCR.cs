using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlants_SCR : MonoBehaviour
{
    [SerializeField] private float turnRange = 10.0f;
    [SerializeField] private float rotSpeed = 0.15f;

    private float startY = 0.0f;
    private float minY = 0.0f;
    private float maxY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        startY = transform.localEulerAngles.y;

        minY = transform.rotation.y - turnRange;
        maxY = transform.rotation.y + turnRange;

        //timeOffset = Random.Range(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(0f, Mathf.SmoothStep(minY, maxY, Mathf.PingPong(Time.time * rotSpeed, 1f)) + startY, 0f);
    }
}
