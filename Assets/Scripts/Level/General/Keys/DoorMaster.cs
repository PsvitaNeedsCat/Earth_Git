using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class DoorMaster : Interactable
{
    [SerializeField] private GameObject[] m_locks;

    private bool m_unlocking = false;

    // Begins the setup for attempting to unlock the door - called when the player interacts with the door
    public override void Invoke()
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
        Debug.Log("Keys to check: " + keysToCheck.Count);
        StartCoroutine(CheckKeyValidity(keysToCheck));
    }

    // Takes the player's keys, performs an animation, and checks if the door can unlock
    private IEnumerator CheckKeyValidity(List<Key> _keys)
    {
        if (_keys.Count <= 0 || _keys.Count < m_locks.Length)
        {
            foreach (Key key in _keys)
            {
                key.m_state = Key.States.collected;
            }
            MessageBus.TriggerEvent(EMessageType.doorLocked);
            yield break;
        }

        // Init changes
        m_unlocking = true;
        m_prompt.SetActive(false);
        Player player = FindObjectOfType<Player>();
        player.GetComponent<PlayerInput>().SetMovement(false);

        int completedTweens = 0;
        int keysRequired = 0;
        bool animationFinished = false;

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
            unlockSeq.OnComplete(() => animationFinished = true);
            unlockSeq.Play();
        }

        while (!animationFinished)
        {
            yield return null;
        }

        
        // Unlock door
        UnlockDoor(ref _keys, ref player);

        // Ending changes
        m_unlocking = false;
        player.GetComponent<PlayerInput>().SetMovement(true);
    }

    // Removes the keys from the player and unlocks (destroys) the door
    private void UnlockDoor(ref List<Key> _keys, ref Player _player)
    {
        for (int i = m_locks.Length - 1; i >= 0; i--)
        {
            _player.m_collectedKeys.Remove(_keys[i].m_keyID);
            _keys[i].RemoveFromUI();
            Destroy(_keys[i].gameObject);
            FindObjectOfType<DoorManager>().UnlockDoor(gameObject.GetInstanceID());
        }

        gameObject.SetActive(false);

        MessageBus.TriggerEvent(EMessageType.doorUnlocked);
    }

    // Tweens the keys back to the player's belt
    private void ReturnKeysToPlayer(ref List<Key> _keys)
    {
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

    // Makes sure not to update the sprite while unlocking the door
    public override void Update()
    {
        if (!m_unlocking)
        {
            base.Update();
        }
    }
}
