using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeHealth : MonoBehaviour
{
    public enum ESegmentType { head, body, tail }
    
    public Color m_damagedColor;
    public List<MeshRenderer> m_segmentRenderers;

    private MaterialPropertyBlock m_damagedMPB;
    static readonly int m_shPropColor = Shader.PropertyToID("_Color");
    private bool[] m_sectionsDamaged = { false, false, false};

    private void Awake()
    {
        m_damagedMPB = new MaterialPropertyBlock();
        m_damagedMPB.SetColor(m_shPropColor, m_damagedColor);
    }

    public void SectionDamaged(ESegmentType _type)
    {
        if (m_sectionsDamaged[(int)_type]) return;

        switch (_type)
        {
            case ESegmentType.head: m_segmentRenderers[0].SetPropertyBlock(m_damagedMPB);
                break;
            case ESegmentType.body:
                m_segmentRenderers[1].SetPropertyBlock(m_damagedMPB);
                m_segmentRenderers[2].SetPropertyBlock(m_damagedMPB);
                m_segmentRenderers[3].SetPropertyBlock(m_damagedMPB);
                m_segmentRenderers[4].SetPropertyBlock(m_damagedMPB);
                m_segmentRenderers[5].SetPropertyBlock(m_damagedMPB);
                break;
            case ESegmentType.tail: m_segmentRenderers[6].SetPropertyBlock(m_damagedMPB);
                break;
        }

        m_sectionsDamaged[(int)_type] = true;
    }
}
