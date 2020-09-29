using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BossHelper
{
    public static IEnumerator ChangeMaterialFloatProperty(Material _material, string _property, float _currentValue, float _endValue, float _rate, bool _increase)
    {
        // Debug.Log("Changing material properties: " + _property + "   " + _currentValue + " " + _endValue + " " + _increase);

        float curValue = _currentValue;

        if (_increase)
        {
            while (_material.GetFloat(_property) < _endValue)
            {
                curValue += _rate * Time.deltaTime;
                _material.SetFloat(_property, curValue);

                yield return null;
            }

            _material.SetFloat(_property, _endValue);
        }
        else
        {
            while (_material.GetFloat(_property) > _endValue)
            {
                curValue += _rate * Time.deltaTime;
                _material.SetFloat(_property, curValue);

                yield return null;
            }

            _material.SetFloat(_property, _endValue);
        }
    }

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
