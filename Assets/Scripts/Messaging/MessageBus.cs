using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

// ENUM NAMES MUST MATCH CORRESPONDING NAME IN RESOUCES FOLDER //
public enum EMessageType
{
    // Music - Must remain at top
    // Add more music to the bottom of the music list
    menuMusic,
    wMusic,
    stopMusic,
    wToadMusic,
    fMusic,
    fCentipedeMusic,
    dMusic,

    none,
    test,
    fadedToBlack,
    fadedToBlackQuiet,
    continueDialogue,

    // Chunk
    chunkDamaged,
    chunkDestroyed,
    chunkHit,
    chunkHitWall,
    chunkRaise,
    waterChunkDestroyed,
    sandLand,
    sandDestroyed,

    // Enemies
    projectileSplash,
    tongueStuck,
    enemySwallow,
    enemySpit,
    grubKilled,
    grubFire,
    enemyTongueExtend,
    tongueEnemyKilled,
    centipedeDamaged,
    centipedeSpawn,
    centipedeTailFire,

    // Player
    playerHurt,
    ting,
    powerRock,
    powerWater,
    powerFire,
    interact,

    // Level
    doorUnlocked,
    doorLocked,
    lavaToStone,
    keyCollected,
    keySpawned,
    pressurePlateOn,
    pressurePlateOff,

    // Blocks
    glassDestroyed,

    // Toad boss
    smallToadJumpInWater,
    swampWave,
    toadDamaged,
    toadJumpInWater,
    toadLand,
    toadRoar,
    toadSpit,
    toadTongue,

    // Added stuff
    fireBugKilled,
}

[System.Serializable]
public class MessageBusEvent : UnityEvent<string>
{

}

public static class MessageBus
{
    private static Dictionary<EMessageType, UnityEvent<string>> m_eventDict = new Dictionary<EMessageType, UnityEvent<string>>();

    private static int check = 0;

    // Subscribes a listener to a type of message
    public static void AddListener(EMessageType _type, UnityAction<string> _listener)
    {
        if (_type == EMessageType.none) { return; }

        // Event already exists
        if (m_eventDict.TryGetValue(_type, out UnityEvent<string> checkEvent))
        {
            checkEvent.AddListener(_listener);
        }
        // Event doesn't exist, create
        else
        {
            UnityEvent<string> newEvent = new MessageBusEvent();
            
            newEvent.AddListener(_listener);
            m_eventDict.Add(_type, newEvent);
        }
    }

    public static void RemoveListener(EMessageType _type, UnityAction<string> _listener)
    {
        if (_type == EMessageType.none) { return; }

        UnityEvent<string> checkEvent = null;

        // Only try to remove listener if event exists
        if (m_eventDict.TryGetValue(_type, out checkEvent))
        {
            checkEvent.RemoveListener(_listener);
        }
    }

    public static void TriggerEvent(EMessageType _type, string _param)
    {
        if (_type == EMessageType.none) { return; }

        UnityEvent<string> triggerEvent = null;

        if (m_eventDict.TryGetValue(_type, out triggerEvent))
        {
            triggerEvent.Invoke(_param);
            Debug.Log("Successfully triggering event " + _type.ToString());
        }
        else
        {
            Debug.Log("Triggering event " + _type.ToString() + " failed");
        }
    }

    public static void TriggerEvent(EMessageType _type)
    {
        if (_type == EMessageType.none) { return; }

        UnityEvent<string> triggerEvent = null;

        if (m_eventDict.TryGetValue(_type, out triggerEvent))
        {
            triggerEvent.Invoke(_type.ToString());
        }
    }
}