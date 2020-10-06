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
}
