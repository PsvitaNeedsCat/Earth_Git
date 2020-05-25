using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    // Returns the closest cardinal direction
    public static Vector3 Cardinal(this Vector3 _vec)
    {
        Vector3 cardinalDir;

        if (Mathf.Abs(_vec.z) > Mathf.Abs(_vec.x))
        {
            if (_vec.z > 0.0f) { cardinalDir = Vector3.forward; }
            else { cardinalDir = Vector3.back; }
        }
        else
        {
            if (_vec.x > 0.0f) { cardinalDir = Vector3.right; }
            else { cardinalDir = Vector3.left; }
        }

        return cardinalDir;
    }
}
