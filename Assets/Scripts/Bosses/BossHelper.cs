using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BossHelper
{
    public static IEnumerator ChangeMaterialFloatProperty(Material _material, string _property, float _currentValue, float _endValue, float _rate, bool _increase)
    {
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
}
