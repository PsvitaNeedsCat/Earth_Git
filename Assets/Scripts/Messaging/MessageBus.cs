using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EMessageType
{
    PlayerPunch,
}

public static class MessageBus
{
    private static Dictionary<EMessageType, System.Action<string>> eventDict = new Dictionary<EMessageType, System.Action<string>>();

    // Subscribes a listener to a type of message
    public static void AddListener(EMessageType _type, System.Action<string> _listener)
    {
        System.Action<string> checkEvent = null;

        // Event already exists
        if (eventDict.TryGetValue(_type, out checkEvent))
        {
            checkEvent += _listener;
        }
        // Event doesn't exist, create
        else
        {
            checkEvent = new System.Action<string>(_listener);
            eventDict.Add(_type, checkEvent);
        }
    }

    public static void RemoveListener(EMessageType _type, System.Action<string> _listener)
    {
        System.Action<string> checkEvent = null;

        // Only try to remove listener if event exists
        if (eventDict.TryGetValue(_type, out checkEvent))
        {
            checkEvent -= _listener;
        }
    }

    public static void TriggerEvent(EMessageType _type, string _data)
    {
        System.Action<string> triggerEvent = null;

        if (eventDict.TryGetValue(_type, out triggerEvent))
        {
            triggerEvent.Invoke(_data);
        }
    }
}

public struct MessageInfo<T>
{
    public EMessageType type;
    public T data;

    public MessageInfo(EMessageType _type, T _data)
    {
        this.type = _type;
        this.data = _data;
    }
}