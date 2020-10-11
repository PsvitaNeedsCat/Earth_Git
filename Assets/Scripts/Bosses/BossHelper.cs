using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BossHelper
{
    public static IEnumerator ChangeMaterialFloatPropertyOver(Material _material, string _property, float _endValue, float _overSeconds)
    {
        float startValue = _material.GetFloat(_property);
        float totalDelta = _endValue - startValue;
        float currentDelta = 0.0f;

        while (Mathf.Abs(currentDelta) <= Mathf.Abs(totalDelta))
        {
            currentDelta += (totalDelta * Time.deltaTime) / _overSeconds;
            _material.SetFloat(_property, startValue + currentDelta);

            yield return null;
        }

        _material.SetFloat(_property, _endValue);
    }

    public static  IEnumerator ChangeMaterialVectorPropertyOver(Material _material, string _property, Vector4 _endValue, float _overSeconds)
    {
        Vector4 startValue = _material.GetVector(_property);
        Vector4 totalDelta = _endValue - startValue;
        Vector4 currentDelta = Vector4.zero;

        while (Mathf.Abs(currentDelta.magnitude) <=  Mathf.Abs(totalDelta.magnitude))
        {
            currentDelta += (totalDelta * Time.deltaTime) / _overSeconds;
            _material.SetVector(_property, startValue + currentDelta);

            yield return null;
        }

        _material.SetVector(_property, _endValue);
    }
}
