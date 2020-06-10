using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CentipedeHealth : MonoBehaviour
{
    public enum ESegmentType { head, body, tail }

    [System.Serializable]
    public struct CentipedeSegmentMaterials
    {
        public ESegmentType m_type;
        public Material m_normal;
        public Material m_heated;
        public Material m_cooled;
    }

    public CentipedeSegmentMaterials[] m_segmentMaterials;
    public GameObject m_crystal;

    public List<MeshRenderer> m_segmentRenderers;
    public GameObject m_bossObject;

    private bool[] m_sectionsActive = { false, false, false };
    private bool[] m_sectionsDamaged = { false, false, false};

    private List<int>[] m_sectionSegments = { new List<int>{ 0 }, new List<int>{ 1, 2, 3, 4, 5 }, new List<int>{ 6 } };

    private void Awake()
    {

    }

    public bool IsSectionDamaged(ESegmentType _type)
    {
        return m_sectionsDamaged[(int)_type];
    }

    public void ActivateSection(bool _activate, ESegmentType _type)
    {
        if (m_sectionsDamaged[(int)_type]) return;

        List<int> segments = m_sectionSegments[(int)_type];

        foreach (int segment in segments)
        {
            m_segmentRenderers[segment].material = (_activate) ? m_segmentMaterials[(int)_type].m_heated : m_segmentMaterials[(int)_type].m_normal;
        }
        
        m_sectionsActive[(int)_type] = _activate;
    }

    public void DamageSection(ESegmentType _type)
    {
        if (!m_sectionsActive[(int)_type]) return;
        if (m_sectionsDamaged[(int)_type]) return;

        List<int> segments = m_sectionSegments[(int)_type];

        foreach (int segment in segments)
        {
            m_segmentRenderers[segment].material = m_segmentMaterials[(int)_type].m_cooled;
            m_segmentRenderers[segment].transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
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
            // m_bossObject.transform.DOScale(0.0f, 1.0f).SetEase(Ease.InElastic).OnComplete(() => Destroy(m_bossObject));
            m_crystal.SetActive(true);
            Destroy(m_bossObject);
        }

        MessageBus.TriggerEvent(EMessageType.lavaToStone);
        MessageBus.TriggerEvent(EMessageType.centipedeDamaged);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);
    }
}
