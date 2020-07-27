using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFreezeManager : MonoBehaviour
{
    public static float s_ogTimeScale = 1.0f;
    public static bool s_frozen = false;

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
        s_ogTimeScale = Time.timeScale;

        Time.timeScale = 0.0f;

        s_frozen = true;

        yield return new WaitForSecondsRealtime(_freezeTime);

        Time.timeScale = s_ogTimeScale;

        s_frozen = false;
    }
}
