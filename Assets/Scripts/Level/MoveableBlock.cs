using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class MoveableBlock : MonoBehaviour
{
    [SerializeField] private Transform m_move_location;
    private Vector3 m_startLocation;
    private GlobalChunkSettings m_chunkSettings;
    private GlobalPlayerSettings m_playerSettings;

    private void Awake()
    {
        m_startLocation = transform.position;
        m_chunkSettings = Resources.Load<GlobalChunkSettings>("ScriptableObjects/GlobalChunkSettings");
        m_playerSettings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
    }

    // Tween to start location
    public void GoToStart()
    {
        Moved(m_startLocation);

        transform.DOKill();
        transform.DOMove(m_startLocation, 0.5f).SetEase(Ease.OutBounce);
    }

    // Goes to the other location
    public void GoToEnd()
    {
        Moved(m_move_location.position);

        transform.DOKill();
        transform.DOMove(m_move_location.position, 0.5f).SetEase(Ease.OutBounce);
    }

    // Called when the block is moved
    private void Moved(Vector3 _centre)
    {
        // Check if there is a chunk in the tile space
        float halfExtents = m_chunkSettings.m_chunkHeight * 0.4f;
        Collider[] hits = Physics.OverlapBox(_centre, new Vector3(halfExtents, halfExtents, halfExtents));

        // If raycast hit something
        if (hits.Length > 0)
        {
            // Check each object it hit
            for (int i = 0; i < hits.Length; i++)
            {
                // Check the parent exists
                Transform parent = hits[i].transform.parent;
                if (!parent) { continue; }

                // Check parent is a chunk
                Chunk chunk = hits[i].transform.parent.GetComponent<Chunk>();
                if (!chunk) { continue; }

                // Hit the chunk
                Vector3 dir = chunk.transform.position - transform.position;
                dir.y = 0.0f;
                dir.Normalize();
                dir = dir.Cardinal();

                chunk.Hit(dir * m_playerSettings.m_chunkHitForce);

                return;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GoToEnd();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GoToStart();
        }
    }
}
