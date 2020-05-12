using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ENUM NAMES MUST MATCH CORRESPONDING NAME IN RESOUCES FOLDER //
public enum EMessageType
{
    test,

    // Chunk
    chunkDamaged,
    chunkDestroyed,
    chunkHit,
    chunkHitWall,
    chunkRaise,

    // Enemies
    projectileSplash,
    tongueStuck,
    enemySwallow,
    enemySpit,
    grubKilled,

    // Player
    playerHurt,
    ting,

    // Toad boss
    smallToadJumpInWater,
    swampWave,
    toadDamaged,
    toadJumpInWater,
    toadLand,
    toadRoar,
    toadSpit,
    toadTongue,
}

public static class MessageBus
{
    private static Dictionary<EMessageType, System.Action<string>> m_eventDict = new Dictionary<EMessageType, System.Action<string>>();

    // Subscribes a listener to a type of message
    public static void AddListener(EMessageType _type, System.Action<string> _listener)
    {
        System.Action<string> checkEvent = null;

        // Event already exists
        if (m_eventDict.TryGetValue(_type, out checkEvent))
        {
            checkEvent += _listener;
        }
        // Event doesn't exist, create
        else
        {
            checkEvent = new System.Action<string>(_listener);
            m_eventDict.Add(_type, checkEvent);
        }
    }

    public static void RemoveListener(EMessageType _type, System.Action<string> _listener)
    {
        System.Action<string> checkEvent = null;

        // Only try to remove listener if event exists
        if (m_eventDict.TryGetValue(_type, out checkEvent))
        {
            checkEvent -= _listener;
        }
    }

    public static void TriggerEvent(EMessageType _type, string _param)
    {
        System.Action<string> triggerEvent = null;

        if (m_eventDict.TryGetValue(_type, out triggerEvent))
        {
            triggerEvent.Invoke(_param);
        }
    }

    public static void TriggerEvent(EMessageType _type)
    {
        System.Action<string> triggerEvent = null;

        if (m_eventDict.TryGetValue(_type, out triggerEvent))
        {
            triggerEvent.Invoke(_type.ToString());
        }
    }
}