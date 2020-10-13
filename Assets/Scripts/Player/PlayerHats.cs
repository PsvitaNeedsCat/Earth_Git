using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum EHatType
{
    none,
    toad,
    potEnemy,
    fireTree,
}

public class PlayerHats : MonoBehaviour
{
    [System.Serializable]
    public struct HatDef
    {
        public EHatType hatType;
        public GameObject hatPrefab;
        public Vector3 positionOverride;
        public Vector3 scaleOverride;
        public Vector3 rotationOverride;
    }

    public List<HatDef> m_hatDefs;
    public Transform m_hatParent;
    private GameObject m_hatObject;

    public void SetHat(EHatType _hatType)
    {
        if (_hatType ==  EHatType.none)
        {
            if (m_hatObject)
            {
                Destroy(m_hatObject);
            }
        }
        else
        {
            // Check for out of bounds  on our list of hat defs
            if ((int)_hatType >= m_hatDefs.Count)
            {
                Debug.LogError("Couldn't find a hat def for that hat type");
                return;
            }
            else
            {
                if (m_hatObject)
                {
                    Destroy(m_hatObject);
                }

                HatDef hatDef = m_hatDefs[(int)_hatType];
                m_hatObject = Instantiate(hatDef.hatPrefab, m_hatParent.position, transform.rotation * Quaternion.Euler(hatDef.rotationOverride), m_hatParent);
                
                m_hatObject.transform.localScale = hatDef.scaleOverride;
                m_hatObject.transform.localPosition = hatDef.positionOverride;

                foreach (Transform child in m_hatObject.transform)
                {
                    foreach(Collider collider in child.GetComponents<Collider>())
                    {
                        Destroy(collider);
                    }
                }
            }
        }
    }
}
