using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CentipedeTargetPoints : MonoBehaviour
{
    public Color drawColor;

    private void OnDrawGizmosSelected()
    {
        foreach (Transform child in transform)
        {
            Gizmos.color = drawColor;
            Gizmos.DrawWireSphere(child.position, 0.2f);

#if UNITY_EDITOR
            Handles.Label(child.position, child.name);
#endif

        }
    }
}
