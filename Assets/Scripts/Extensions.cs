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

    public static Vector3 RelativeDirection2(this Camera _camera, Vector2 _direction)
    {
        // Get yaw
        float yaw = _camera.transform.rotation.eulerAngles.y;

        // Convert direction to 3D
        Vector3 dir = new Vector3(_direction.x, 0.0f, _direction.y);

        // Rotate direction vector by yaw
        dir = Quaternion.Euler(new Vector3(0.0f, yaw, 0.0f)) * dir;

        return dir;
    }
}
