using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGradientScript : MonoBehaviour
{
    private MeshRenderer m_meshRenderer;

    public Gradient m_gradient1;
    public Gradient m_gradient2;

    public float m_speed = 0.5f;
    private float m_gradientTime = 0.0f;

    private Color m_colour1;
    private Color m_colour2;

    // Start is called before the first frame update
    void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_gradientTime = Mathf.PingPong(Time.time * m_speed, 1);

        m_colour1 = (m_gradient1.Evaluate(m_gradientTime));
        m_meshRenderer.material.SetColor("_Color", m_colour1);

        m_colour2 = (m_gradient2.Evaluate(m_gradientTime));
        m_meshRenderer.material.SetColor("_Color1", m_colour2);
    }
}
