using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Manages the health of the centipede, and the changing of segment materials
public class CentipedeHealth : MonoBehaviour
{
    public enum ESegmentType { head, body, tail }

    // Struct to hold the different materials for each segment
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

    public List<SkinnedMeshRenderer> m_segmentRenderers;
    public GameObject m_bossObject;

    private bool[] m_sectionsActive = { false, false, false };
    private bool[] m_sectionsDamaged = { false, false, false};
    private CentipedeTrainAttack m_trainAttack;

    // Array of lists of segments indices: "Head segments", "Body segments", "Tail segments"
    private List<int>[] m_sectionSegments = { new List<int>{ 0 }, new List<int>{ 1, 2, 3, 4, 5 }, new List<int>{ 6 } };

    private void Awake()
    {
        m_trainAttack = GetComponent<CentipedeTrainAttack>();
    }

    public bool IsSectionDamaged(ESegmentType _type)
    {
        return m_sectionsDamaged[(int)_type];
    }

    // Activate (or deactivate) a section of the body, changing all materials
    public void ActivateSection(bool _activate, ESegmentType _type)
    {
        Debug.Log("Activating section " + _type.ToString() + " with value: " + _activate);

        // Don't activate a section if it's already damaged
        if (m_sectionsDamaged[(int)_type]) return;

        List<int> segments = m_sectionSegments[(int)_type];

        // Change the material of each segment
        foreach (int segment in segments)
        {
            m_segmentRenderers[segment].material = (_activate) ? m_segmentMaterials[(int)_type].m_heated : m_segmentMaterials[(int)_type].m_normal;
        }
        
        // Store the new state of this section
        m_sectionsActive[(int)_type] = _activate;
    }

    // Damages a section of the centipede, storing the new state, and changing the materials
    public void DamageSection(ESegmentType _type)
    {
        Debug.Log("Trying to damage section " + _type.ToString());

        // If the section is not active, or has already been damaged, it can't be damaged
        if (!m_sectionsActive[(int)_type]) return;
        if (m_sectionsDamaged[(int)_type]) return;

        List<int> segments = m_sectionSegments[(int)_type];

        // Change material of segments
        foreach (int segment in segments)
        {
            m_segmentRenderers[segment].material = m_segmentMaterials[(int)_type].m_cooled;
            m_segmentRenderers[segment].transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
        }

        if (_type == ESegmentType.head) { m_trainAttack.OnDamaged(); }

        m_sectionsDamaged[(int)_type] = true;

        // Check if any sections of the body are still alive
        bool anyAlive = false;
        foreach (bool section in m_sectionsDamaged)
        {
            if (!section)
            {
                anyAlive = true;
            }
        }

        // If no sections are alive, die
        if (!anyAlive)
        {
            m_crystal.SetActive(true);
            Destroy(m_bossObject);
        }

        MessageBus.TriggerEvent(EMessageType.lavaToStone);
        MessageBus.TriggerEvent(EMessageType.centipedeDamaged);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);
    }
}
