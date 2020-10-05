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
        public Texture m_cooled;
    }

    public CentipedeSegmentMaterials[] m_segmentMaterials;
    public GameObject[] m_segmentDamagedEffects;
    public GameObject m_crystal;
    public List<GameObject> m_healthIcons;

    public List<SkinnedMeshRenderer> m_segmentRenderers;
    public GameObject m_bossObject;

    [SerializeField] private ParticleSystem[] m_shieldBreakParticles = new ParticleSystem[7];
    [SerializeField] private ParticleSystem[] m_shieldRepairParticles = new ParticleSystem[7];

    private bool[] m_sectionsActive = { false, false, false, false, false, false, false };
    private bool[] m_sectionsDamaged = { false, false, false};
    private CentipedeTrainAttack m_trainAttack;

    // Array of lists of segments indices: "Head segments", "Body segments", "Tail segments"
    private List<int>[] m_sectionSegments = { new List<int> { 0 }, new List<int> { 1, 2, 3, 4, 5 }, new List<int> { 6 } };

    private void Awake()
    {
        m_trainAttack = GetComponent<CentipedeTrainAttack>();
    }

    public bool IsSectionDamaged(ESegmentType _type)
    {
        return m_sectionsDamaged[(int)_type];
    }

    public int GetHealth()
    {
        int currentHealth = 0;

        for (int i = 0; i < m_sectionsDamaged.Length; i++)
        {
            if (!m_sectionsDamaged[i])
            {
                currentHealth++;
            }    
        }

        return currentHealth;
    }

    public List<int> GetDamagedSegments()
    {
        List<int> damagedSegments = new List<int>();

        for (int i = 0; i < m_sectionsDamaged.Length; i++)
        {
            if (m_sectionsDamaged[i])
            {
                damagedSegments.Add(i);
            }
        }

        return damagedSegments;
    }

    // Activate (or deactivate) a section of the body, changing all materials
    public void ActivateSection(bool _activate, int _sectionIndex)
    {
        // Don't activate a section if it's already damaged
        if (m_sectionsDamaged[(int)IndexToSegmentType(_sectionIndex)])
        {
            return;
        }

        // Debug.Log("Activated section " + _sectionIndex);

        // Change the segment's material
        // m_segmentRenderers[_sectionIndex].material = (_activate) ? m_segmentMaterials[(int)IndexToSegmentType(_sectionIndex)].m_heated : m_segmentMaterials[(int)IndexToSegmentType(_sectionIndex)].m_normal;
        if (_activate)
        {
            StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_segmentRenderers[_sectionIndex].material, "_TextureBlend", 0.0f, 1.0f, 2.5f, true));
            StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_segmentRenderers[_sectionIndex].material, "_Cutoff", 0.8f, 1.1f, 0.3f, true));
            StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_segmentRenderers[_sectionIndex].material, "_FresnelStrength", 5.0f, 20.0f, 15.0f, true));
            m_shieldBreakParticles[_sectionIndex].Play();
        }
        else
        {
            StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_segmentRenderers[_sectionIndex].material, "_TextureBlend", 1.0f, 0.0f, -2.5f, false));
            StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_segmentRenderers[_sectionIndex].material, "_Cutoff", 1.1f, 0.8f, -0.3f, false));
            StartCoroutine(BossHelper.ChangeMaterialFloatProperty(m_segmentRenderers[_sectionIndex].material, "_FresnelStrength", 20.0f, 5.0f, -15.0f, false));
            m_shieldRepairParticles[_sectionIndex].Play();
        }
        
        
        // Store the new state of this section
        m_sectionsActive[_sectionIndex] = _activate;
    }

    // Damages a section of the centipede, storing the new state, and changing the materials
    public void DamageSection(ESegmentType _type)
    {
        // Debug.Log("Trying to damage section " + _type.ToString());


        // If the section is not active, or has already been damaged, it can't be damaged
        if (!m_sectionsActive[SegmentTypeToIndex(_type)])
        {
            return;
        }
        if (m_sectionsDamaged[(int)_type])
        {
            return;
        }

        // Debug.Log("Damaged section " + _type.ToString());

        m_healthIcons[0].transform.parent.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        m_healthIcons[(int)_type].SetActive(false);

        HitFreezeManager.BeginHitFreeze(0.1f);

        List<int> segments = m_sectionSegments[(int)_type];

        // Change material of segments
        for (int i = 0; i < segments.Count; i++)
        {
            int segmentIndex = segments[i];
            // m_segmentRenderers[segmentIndex].material = m_segmentMaterials[(int)_type].m_cooled;

            StopAllCoroutines();

            m_segmentRenderers[segmentIndex].material.SetTexture("_MainTex", m_segmentMaterials[(int)_type].m_cooled);
            m_segmentRenderers[segmentIndex].material.SetFloat("_TextureBlend", 0.0f);
            m_segmentRenderers[segmentIndex].material.SetFloat("_FresnelMultiplier", 1.0f);
            m_segmentRenderers[segmentIndex].material.SetFloat("_Cutoff", 0.8f);

            m_segmentRenderers[segmentIndex].transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            m_segmentDamagedEffects[i].SetActive(true);
        }

        if (_type == ESegmentType.head)
        {
            m_trainAttack.OnDamaged(); 
        }

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
            m_crystal.GetComponentInChildren<Crystal>().Collected(FindObjectOfType<Player>());
            Destroy(m_bossObject);
        }

        MessageBus.TriggerEvent(EMessageType.lavaToStone);
        MessageBus.TriggerEvent(EMessageType.centipedeDamaged);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);
    }

    private int SegmentTypeToIndex(ESegmentType _type)
    {
        if (_type == ESegmentType.head)
        {
            return 0;
        }

        if (_type == ESegmentType.tail)
        {
            return 6;
        }

        return 1;
    }

    // Converts segment index to segment type
    private ESegmentType IndexToSegmentType(int _index)
    {
        if (_index == 0)
        {
            return ESegmentType.head;
        }

        if (_index == 6)
        {
            return ESegmentType.tail;
        }

        return ESegmentType.body;
    }

    
}
