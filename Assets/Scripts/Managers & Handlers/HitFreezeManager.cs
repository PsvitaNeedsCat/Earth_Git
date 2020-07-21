using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFreezeManager : MonoBehaviour
{
    private static HitFreezeManager s_instance = null;

    private void Awake()
    {
        if (s_instance && s_instance != this)
        {
            Destroy(s_instance.gameObject);
        }

        s_instance = this;
    }

    public static void BeginHitFreeze(float _freezeTime)
    {
        s_instance.StartCoroutine(FreezeForTime(_freezeTime));
    }

    private static IEnumerator FreezeForTime(float _freezeTime)
    {
        float ogTimeScale = Time.timeScale;

        Time.timeScale = 0.0f;

        yield return new WaitForSecondsRealtime(_freezeTime);

        Time.timeScale = ogTimeScale;
    }
}
