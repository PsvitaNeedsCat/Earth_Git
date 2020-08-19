using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class DoorMaster : MonoBehaviour
{
    [SerializeField] private GameObject[] m_locks;

    // When the player collides with the door, take all their keys and pass it to a function to check if the door can unlock
    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player)
        {
            Key[] keys = FindObjectsOfType<Key>();
            List<Key> keysToCheck = new List<Key>();
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].m_state == Key.States.collected)
                {
                    keysToCheck.Add(keys[i]);
                }
            }

            StartCoroutine(CheckKeyValidity(keysToCheck));
        }
    }

    // Takes the player's keys, performs an animation, and checks if the door can unlock
    private IEnumerator CheckKeyValidity(List<Key> _keys)
    {
        bool valid = _keys.Count >= m_locks.Length;
        int completedTweens = 0;

        // Animation
        for (int i = 0; i < m_locks.Length; i++)
        {
            if (i > _keys.Count - 1)
            {
                break;
            }

            _keys[i].PauseAnimation();

            _keys[i].transform.rotation = m_locks[i].transform.rotation;
            _keys[i].transform.rotation = Quaternion.Euler(_keys[i].transform.rotation.eulerAngles + new Vector3(0.0f, 90.0f, 90.0f));
            Sequence unlockSeq = DOTween.Sequence();
            unlockSeq.Append(_keys[i].transform.DOMove(m_locks[i].transform.position, 0.5f));

            Vector3 rotationVector = _keys[i].transform.rotation.eulerAngles;
            rotationVector.x += 90.0f;
            Vector3 initRotationVec = _keys[i].transform.rotation.eulerAngles;
            unlockSeq.Append(_keys[i].transform.DORotate(rotationVector, 1.0f));
            unlockSeq.Append(_keys[i].transform.DORotate(initRotationVec, 1.0f));

            unlockSeq.OnComplete(() => ++completedTweens);
            unlockSeq.Play();
        }

        while (completedTweens < _keys.Count)
        {
            yield return null;
        }

        Player player = FindObjectOfType<Player>();
        if (valid)
        {
            // Unlock door
            for (int i = m_locks.Length - 1; i >= 0; i--)
            {
                --player.m_numKeys;
                Destroy(_keys[i].gameObject);
                Destroy(gameObject);
            }
        }
        else
        {
            // Return keys to player
            for (int i = 0; i < _keys.Count; i++)
            {
                _keys[i].transform.DORotate(new Vector3(0.0f, 0.0f, 0.0f), 0.5f);
                Vector3 tweenPos = player.transform.position;
                tweenPos.y += 1.2f;
                _keys[i].transform.DOMove(tweenPos, 0.5f);
                _keys[i].ContinueAnimation();
            }
        }
    }
}
