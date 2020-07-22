using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlippableTile : MonoBehaviour
{
    public Tile m_topTile;
    public Tile m_bottomTile;
    public float m_flipTime;

    public void Flip()
    {
        StartCoroutine(StartFlip());
    }

    private IEnumerator StartFlip()
    {
        // Disable functionality of top tile, store temp information
        Tile tempTile = m_topTile;
        EChunkType tempType = tempTile.GetTileType();
        tempTile.SetChunkType(m_bottomTile.GetTileType());
        tempTile.SetCollider(false);
        tempTile.SetIgnore(true);
        m_bottomTile.SetCollider(true);

        // Rotate tile
        transform.DOBlendableRotateBy(Vector3.right * 180.0f, m_flipTime);
        yield return new WaitForSeconds(m_flipTime);

        // Swap tiles, make new top tile functional
        m_topTile = m_bottomTile;
        m_topTile.SetIgnore(false);
        m_bottomTile = tempTile;
        m_bottomTile.SetChunkType(tempType);
    }
}
