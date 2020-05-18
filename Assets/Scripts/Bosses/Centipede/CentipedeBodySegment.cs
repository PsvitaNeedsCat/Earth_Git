using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBodySegment : MonoBehaviour
{
    public Color m_damagedColor;
    public CentipedeLaser m_laser;

    private MeshRenderer m_meshRenderer;
    private MaterialPropertyBlock m_damagedMPB;
    private bool m_cooledDown = false;

    static readonly int m_shPropColor = Shader.PropertyToID("_Color");

    private void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_damagedMPB = new MaterialPropertyBlock();
        m_damagedMPB.SetColor(m_shPropColor, m_damagedColor);
    }

    public void CoolDown()
    {
        m_meshRenderer.SetPropertyBlock(m_damagedMPB);
        m_cooledDown = true;
        CentipedeBoss.SegmentDied();
    }

    public void FireLaserFor(float _duration)
    {
        m_laser.FireLaserFor(_duration);
    }
}
