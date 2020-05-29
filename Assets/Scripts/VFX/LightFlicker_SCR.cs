using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LightFlicker_SCR : MonoBehaviour
{
    private Light thisLight;

    [SerializeField] private Gradient gradient;
    [SerializeField] private float flickerTime = 0.0f;
    [SerializeField] private float minBrightness = 0.0f;
    [SerializeField] private float maxBrightness = 0.0f;
    private float curFlickTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        thisLight = GetComponent<Light>();
        curFlickTime = flickerTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (curFlickTime > 0)
        {
            curFlickTime -= Time.deltaTime;
        }
        else
        {
            thisLight.intensity = Random.Range(minBrightness, maxBrightness);
            thisLight.color = gradient.Evaluate(Random.Range(0, 1));
            curFlickTime = flickerTime;
        }
    }
}
