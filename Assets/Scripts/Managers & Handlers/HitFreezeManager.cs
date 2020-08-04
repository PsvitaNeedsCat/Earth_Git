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

    // Freezes game for a set amount of tiem
    public static void BeginHitFreeze(float _freezeTime)
    {
        s_instance.StartCoroutine(FreezeForTime(_freezeTime));
    }

    // Keeps the game frozen until the time is doen
    private static IEnumerator FreezeForTime(float _freezeTime)
    {
        // Become frozen
        s_ogTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        s_frozen = true;

        // Wait
        yield return new WaitForSecondsRealtime(_freezeTime);

        // Unfreeze
        Time.timeScale = s_ogTimeScale;
        s_frozen = false;
    }
}
