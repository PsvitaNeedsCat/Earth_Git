using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraEyes : MonoBehaviour
{
    public float m_blinkSpeed = 0.03f;
    public float m_blinkDelayLower;
    public float m_blinkDelayUpper;
    public float m_eyeClosedTime = 0.25f;

    private Renderer m_renderer;

    private readonly int m_blinkFrames = 3;

    private void OnEnable()
    {
        m_renderer = GetComponentInChildren<Renderer>();
        StartCoroutine(Blink());
    }

    public void Fade(bool _in, float _overSeconds)
    {
        Vector4 currentColour = m_renderer.material.GetColor("_Color");
        Vector4 endColour = currentColour;

        if (_in)
        {
            endColour.w = 1.0f;
        }
        else
        {
            endColour.w = 0.0f;
        }

        StartCoroutine(BossHelper.ChangeMaterialVectorPropertyOver(m_renderer.material, "_Color", endColour, _overSeconds));
    }

    private IEnumerator Blink()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(m_blinkDelayLower, m_blinkDelayUpper));

            for (int i = 0; i < m_blinkFrames; i++)
            {
                yield return new WaitForSeconds(m_blinkSpeed * Time.deltaTime);
                SetFrame(i);
            }

            yield return new WaitForSeconds(m_eyeClosedTime);

            for (int j = m_blinkFrames; j >= 0;  j--)
            {
                yield return new WaitForSeconds(m_blinkSpeed * Time.deltaTime);
                SetFrame(j);
            }
        }
    }

    private void SetFrame(int _frameIndex)
    {
        Vector2 offset = new Vector2(_frameIndex * (1.0f / m_blinkFrames), 0);
        m_renderer.material.SetTextureOffset("_MainTex", offset);
        Debug.Log("Setting eyes to frame: " + _frameIndex);
    }
}
