using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [HideInInspector] public bool[] m_isDoorUnlocked;
    [HideInInspector] public Dictionary<int, bool> m_collectedKeys = new Dictionary<int, bool>();
    [SerializeField] private GameObject[] m_doors = new GameObject[] { };

    private void Awake()
    {
        m_isDoorUnlocked = new bool[m_doors.Length];
    }

    // Called by save manager - updates the unlocked doors and unlocks them accordingly
    public void Init(bool[] _unlockedDoors, Dictionary<int, bool> _prevCollectedKeys)
    {
        m_isDoorUnlocked = _unlockedDoors;

        for (int i = 0; i < m_isDoorUnlocked.Length; i++)
        {
            if (m_isDoorUnlocked[i])
            {
                m_doors[i].GetComponent<DoorMaster>().UnlockDoorSilent();
            }
        }

        foreach (KeyValuePair<int, bool> i in _prevCollectedKeys)
        {
            if (m_collectedKeys.ContainsKey(i.Key))
            {
                m_collectedKeys[i.Key] = i.Value;
            }
            else
            {
                m_collectedKeys.Add(i.Key, i.Value);
            }
        }
    }

    // Checks the list of doors for the given instance ID and sets it as unlocked
    public void UnlockDoor(int _instanceId)
    {
        for (int i = 0; i < m_doors.Length; i++)
        {
            if (m_doors[i].GetInstanceID() == _instanceId)
            {
                m_isDoorUnlocked[i] = true;
                break;
            }
        }
    }

    // Adds a key to the door manager reference
    public void AddKey(int _id, bool _collected = false)
    {
        if (!m_collectedKeys.ContainsKey(_id))
        {
            m_collectedKeys.Add(_id, _collected);
        }
    }

    // Checks if a key with a specific ID has been collected - used to destroy previously collected keys
    public bool HasKeyBeenCollected(int _id)
    {
        if (!m_collectedKeys.ContainsKey(_id))
        {
            return false;
        }

        return m_collectedKeys[_id];
    }

    // Called when a key has been collected - sets it as so
    public void CollectedKey(int _id)
    {
        if (!m_collectedKeys.ContainsKey(_id))
        {
            Debug.LogError("Cannot set collected key:" + _id);
            return;
        }

        m_collectedKeys[_id] = true;
    }
}
