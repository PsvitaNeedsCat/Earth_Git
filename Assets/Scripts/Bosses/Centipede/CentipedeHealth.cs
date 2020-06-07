using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CentipedeHealth : MonoBehaviour
{
    public enum ESegmentType { head, body, tail }
    
    public Color m_damagedColor;
    public List<MeshRenderer> m_segmentRenderers;
    public GameObject m_bossObject;

    private MaterialPropertyBlock m_damagedMPB;
    static readonly int m_shPropColor = Shader.PropertyToID("_Color");
    private bool[] m_sectionsDamaged = { false, false, false};

    private void Awake()
    {
        m_damagedMPB = new MaterialPropertyBlock();
        m_damagedMPB.SetColor(m_shPropColor, m_damagedColor);
    }

    public bool IsSectionDamaged(ESegmentType _type)
    {
        return m_sectionsDamaged[(int)_type];
    }

    public void DamageSection(ESegmentType _type)
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

        bool anyAlive = false;

        foreach (bool section in m_sectionsDamaged)
        {
            if (!section)
            {
                anyAlive = true;
            }
        }

        if (!anyAlive)
        {
            m_bossObject.transform.DOScale(0.0f, 1.0f).SetEase(Ease.InElastic).OnComplete(() => Destroy(m_bossObject));
        }
    }
}
