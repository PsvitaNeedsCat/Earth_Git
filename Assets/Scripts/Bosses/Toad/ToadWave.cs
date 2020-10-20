using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToadWave : MonoBehaviour
{
    public float m_travelTime = 3.0f;
    public float m_travelDist = 12.0f;
    public AnimationCurve m_waveHeightCurve;

    public void Launch()
    {
        MessageBus.TriggerEvent(EMessageType.swampWave);

        ActivateChildren();

        transform.DOMoveZ(transform.position.z + m_travelDist, m_travelTime).OnComplete(() => WaveEnd());
        transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);

        // transform.DOPunchScale(new Vector3(0.0f, 2.0f, 0.0f), m_travelTime, 0, 0).SetEase(Ease.InOutCubic);
        StartCoroutine(SetWaveHeight());
    }

    private IEnumerator SetWaveHeight()
    {
        float timer = 0.0f;

        while (timer < m_travelTime)
        {
            timer += Time.deltaTime;

            float sampleTime = Mathf.Clamp01(timer / m_travelTime);
            Vector3 localScale = transform.localScale;
            localScale.y = m_waveHeightCurve.Evaluate(sampleTime);
            transform.localScale = localScale;

            yield return null;
        }
    }

    void WaveEnd()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<ToadWaveSegment>().Disable();
        }
    }

    void ActivateChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
