using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeSprite : MonoBehaviour
{
    private SpriteRenderer m_renderer = null;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
    }

    // Fades the sprite in
    public void FadeIn(float _duration)
    {
        m_renderer.DOFade(1.0f, _duration);
    }

    public void FadeOut(float _duration)
    {
        m_renderer.DOFade(0.0f, _duration);
    }
}
