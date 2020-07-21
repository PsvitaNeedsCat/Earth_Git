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
        Tile tempTile = m_topTile;
        eChunkType tempType = tempTile.GetTileType();
        tempTile.SetChunkType(m_bottomTile.GetTileType());
        tempTile.SetCollider(false);
        tempTile.SetIgnore(true);
        m_bottomTile.SetCollider(true);

        transform.DOBlendableRotateBy(Vector3.right * 180.0f, m_flipTime);

        yield return new WaitForSeconds(m_flipTime);

        m_topTile = m_bottomTile;
        m_topTile.SetIgnore(false);
        m_bottomTile = tempTile;
        m_bottomTile.SetChunkType(tempType);
    }
}
