using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class FadeSprite : MonoBehaviour
{
    private List<SpriteRenderer> m_renderers;

    private void Awake()
    {
        m_renderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
    }

    // Fades the sprite in
    public void FadeIn(float _duration)
    {
        foreach (SpriteRenderer renderer in m_renderers)
        {
            renderer.DOFade(1.0f, _duration);
        }
    }

    public void FadeOut(float _duration)
    {
        foreach (SpriteRenderer renderer in m_renderers)
        {
            renderer.DOFade(0.0f, _duration);
        }
    }
}
