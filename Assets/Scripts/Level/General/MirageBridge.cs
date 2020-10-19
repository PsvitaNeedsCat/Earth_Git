using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MirageBridge : MonoBehaviour
{
    private Renderer m_renderer;
    [SerializeField] private GameObject m_flowers;

    private void OnEnable()
    {
        m_renderer = GetComponent<Renderer>();

        StartCoroutine(AppearSequence());
    }

    private IEnumerator AppearSequence()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (Transform child in m_flowers.transform)
        {
            child.localScale = Vector3.zero;
        }

        StartCoroutine(BossHelper.ChangeMaterialFloatPropertyOver(m_renderer.material, "_Cutoff", 0.0f, 2.0f));

        yield return new WaitForSeconds(1.0f);

        foreach (Transform child in m_flowers.transform)
        {
            child.DOScale(Vector3.one, 1.5f);
        }
    }
}
