using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Rotation : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        bool m_rotating = false;

        while (true)
        {
            if (!m_rotating)
            {
                m_rotating = true;
                transform.DORotate(new Vector3(0.0f, 360.0f, 0.0f), 3.0f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).OnComplete(() => m_rotating = false);
            }

            yield return null;
        }
    }
}
