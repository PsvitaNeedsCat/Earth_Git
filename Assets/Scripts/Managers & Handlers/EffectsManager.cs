using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EffectsManager : MonoBehaviour
{
    public enum EEffectType
    {
        rockSummon,
        fieryExplosion,
    }

    private readonly string m_effectsPath = "Effects";
    private static Dictionary<string, GameObject> s_effectDictionary = new Dictionary<string, GameObject>();
    private static Transform s_transform;

    private void Awake()
    {
        GameObject[] effects = Resources.LoadAll(m_effectsPath, typeof(GameObject)).Cast<GameObject>().ToArray();

        for (int i = 0; i < effects.Length; i++)
        {
            if (!s_effectDictionary.ContainsKey(effects[i].name))
            {
                s_effectDictionary.Add(effects[i].name, effects[i]);
            }
        }
    }

    private void OnEnable()
    {
        s_transform = transform;
    }
    private void OnDisable()
    {
        s_transform = null;
    }

    // Create an instance of the specified type of effect, and returns a reference to the object created
    public static GameObject SpawnEffect(EEffectType _type, Vector3 _position, Quaternion _rotation, Vector3 _scale, float _destroyAfter)
    {
        if (s_transform = null)
        {
            Debug.LogError("Tried to spawn an effect without an instance of effects manager, please place the prefab in the scene");
            return null;
        }

        // Try to find effect in dictionary
        GameObject effectPrefab = s_effectDictionary[_type.ToString()];

        // If not found, log an error and return
        if (!effectPrefab)
        {
            Debug.LogError("Effect could not be found with name " + _type.ToString());
            return null;
        }

        // Create effect, and modify transform
        GameObject newEffect = Instantiate(effectPrefab, s_transform);
        Destroy(newEffect, _destroyAfter);
        newEffect.transform.position = _position;
        newEffect.transform.rotation = _rotation;
        newEffect.transform.localScale = _scale;

        return newEffect;
    }
}
