using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuGradientScript : MonoBehaviour
{
    private Image m_image = null;

    [FormerlySerializedAs("m_gradient1")]
    public Gradient m_gradient;

    public float m_speed = 0.5f;
    private float m_gradientTime = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        m_image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        m_gradientTime = Mathf.PingPong(Time.time * m_speed, 1);

        m_image.color = m_gradient.Evaluate(m_gradientTime);
    }
}
