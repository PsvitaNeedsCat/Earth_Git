using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileArrow : MonoBehaviour
{
    [SerializeField] private Player m_player = null;

    private GlobalPlayerSettings m_settings = null;

    public void SetDirection(Vector2 _aimDirection)
    {
        if (!m_settings)
        {
            m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
        }

        // Move to position
        Vector3 direction = Camera.main.RelativeDirection2(_aimDirection.normalized) * m_settings.m_arrowMoveDist;

        transform.position = m_player.transform.position + direction;

        Vector3 rotDirection = m_player.transform.position - transform.position;
        if (rotDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rotDirection);
        }
    }
}
