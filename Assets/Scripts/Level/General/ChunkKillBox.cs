using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ChunkKillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Chunk chunk = other.GetComponent<Chunk>();
        if (chunk)
        {
            PrepareToFall(chunk);
        }
    }

    // Gets the chunk to fall downwards and then destroy
    private void PrepareToFall(Chunk _chunk)
    {
        GameObject chunkObj = _chunk.gameObject;
        Destroy(_chunk);
        Destroy(chunkObj.GetComponent<Rigidbody>());

        // Tween
        Vector3 tweenPos = chunkObj.transform.position;
        Vector3 diff = transform.position - tweenPos;
        diff.y = 0.0f;
        tweenPos += diff;
        chunkObj.transform.DOMove(tweenPos, 0.25f).OnComplete(() => Fall(chunkObj));
    }

    // Makes the chunk fall down fast and then destroys it
    private void Fall(GameObject _chunk)
    {
        Vector3 sinkPosition = transform.position;
        sinkPosition.y -= 5.0f;
        _chunk.transform.DOMove(sinkPosition, 0.4f).SetEase(Ease.InCirc).OnComplete(() => Destroy(_chunk));
    }
}
