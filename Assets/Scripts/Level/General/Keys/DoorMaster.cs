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
                    keys[i].m_state = Key.States.unlocking;
                    keysToCheck.Add(keys[i]);
                }
            }

            StartCoroutine(CheckKeyValidity(keysToCheck));
        }
    }

    // Takes the player's keys, performs an animation, and checks if the door can unlock
    private IEnumerator CheckKeyValidity(List<Key> _keys)
    {
        if (_keys.Count <= 0)
        {
            yield break;
        }

        bool valid = _keys.Count >= m_locks.Length;
        int completedTweens = 0;
        int keysRequired = 0;

        // Animation
        for (int i = 0; i < m_locks.Length; i++)
        {
            if (i > _keys.Count - 1)
            {
                break;
            }

            ++keysRequired;

            // Align with lock
            _keys[i].transform.parent = null;
            _keys[i].transform.rotation = m_locks[i].transform.rotation;
            _keys[i].transform.rotation = Quaternion.Euler(_keys[i].transform.rotation.eulerAngles + new Vector3(0.0f, 90.0f, 90.0f));
            Sequence unlockSeq = DOTween.Sequence();
            unlockSeq.Append(_keys[i].transform.DOMove(m_locks[i].transform.position, 0.5f).OnComplete(() => AudioManager.Instance.PlaySound("unlocking")));
            unlockSeq.Insert(0.0f, _keys[i].transform.DOScale(1.0f, 0.4f));

            // Unlocking animation
            Vector3 rotationVector = new Vector3(0.0f, 90.0f, 0.0f);
            Vector3 returnVector = new Vector3(0.0f, -90.0f, 0.0f);
            unlockSeq.Append(_keys[i].transform.DORotate(rotationVector, 1.0f, RotateMode.LocalAxisAdd));
            unlockSeq.Append(_keys[i].transform.DORotate(returnVector, 1.0f, RotateMode.LocalAxisAdd));

            // Return
            unlockSeq.OnComplete(() => ++completedTweens);
            unlockSeq.Play();
        }

        while (completedTweens < keysRequired)
        {
            yield return null;
        }

        Player player = FindObjectOfType<Player>();
        if (valid)
        {
            // Unlock door
            for (int i = m_locks.Length - 1; i >= 0; i--)
            {
                player.m_collectedKeys.Remove(_keys[i].m_keyID);
                Destroy(_keys[i].gameObject);
                FindObjectOfType<DoorManager>().UnlockDoor(gameObject.GetInstanceID());
                gameObject.SetActive(false);
            }

            FindObjectOfType<KeyUI>().UpdateIcons();

            MessageBus.TriggerEvent(EMessageType.doorUnlocked);
        }
        else
        {
            // Return keys to player
            for (int i = 0; i < _keys.Count; i++)
            {
                _keys[i].m_state = Key.States.collected;
                _keys[i].transform.parent = _keys[i].m_beltLocation.transform;
                _keys[i].transform.DORotateQuaternion(Quaternion.identity, 0.5f);
                _keys[i].transform.DOScale(0.1f, 0.4f);
                _keys[i].transform.DOLocalMove(Vector3.zero, 0.5f);
            }

            MessageBus.TriggerEvent(EMessageType.doorLocked);
        }
    }
}
