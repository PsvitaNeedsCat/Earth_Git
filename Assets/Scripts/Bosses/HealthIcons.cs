using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class HealthIcons : MonoBehaviour
{
    [SerializeField] private GameObject[] m_healthForegrounds = new GameObject[] { };

    // Makes the health icons invisible at start
    private void Awake()
    {
        for (int i = 0; i < m_healthForegrounds.Length; i++)
        {
            m_healthForegrounds[i].transform.localScale = Vector3.zero;
        }
    }

    // Animates in the health icons uisng tweening
    public void AnimateInIcons()
    {
        Sequence animation = DOTween.Sequence();

        for (int i = 0; i < m_healthForegrounds.Length; i++)
        {
            animation.Insert(0.25f * i, m_healthForegrounds[i].transform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBounce));
        }

        animation.Play();
    }
}
