using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EMessageType
{
    PlayAudio,

}

public static class MessageBus
{
    private static Dictionary<EMessageType, UnityEvent> eventDict = new Dictionary<EMessageType, UnityEvent>();

    // Subscribes a listener to a type of message
    public static void AddListener(EMessageType _type, UnityAction _listener)
    {
        UnityEvent checkEvent = null;

        // Event already exists
        if (eventDict.TryGetValue(_type, out checkEvent))
        {
            checkEvent.AddListener(_listener);
        }
        // Event doesn't exist, create
        else
        {
            checkEvent = new UnityEvent();
            checkEvent.AddListener(_listener);
            eventDict.Add(_type, checkEvent);
        }
    }

    public static void RemoveListener(EMessageType _type, UnityAction _listener)
    {
        UnityEvent checkEvent = null;

        // Only try to remove listener if event exists
        if (eventDict.TryGetValue(_type, out checkEvent))
        {
            checkEvent.RemoveListener(_listener);
        }
    }

    public static void TriggerEvent(EMessageType _type)
    {
        UnityEvent triggerEvent = null;

        if (eventDict.TryGetValue(_type, out triggerEvent))
        {
            triggerEvent.Invoke();
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