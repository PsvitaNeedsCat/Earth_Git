using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

// ENUM NAMES MUST MATCH CORRESPONDING NAME IN RESOUCES FOLDER //
public enum EMessageType
{
    none,
    test,
    fadedToBlack,
    fadedToBlackQuiet,
    continueDialogue,
    teleportPlayer,

    // Chunk
    chunkDamaged,
    chunkDestroyed,
    chunkHit,
    chunkHitWall,
    chunkRaise,
    waterChunkDestroyed,
    sandLand,
    sandDestroyed,
    chunkSinking,
    fieryExplosion,

    // Enemies
    projectileSplash,
    tongueStuck,
    enemySwallow,
    enemySpit,
    grubFire,
    enemyTongueExtend,
    tongueEnemyKilled,
    centipedeDamaged,
    centipedeSpawn,
    centipedeTailFire,
    sandProjectileDestroyed,
    potDestroyed,
    centipedeBodyFire,
    playerSpotted,
    chargingUp,

    // Player
    playerHurt,
    ting,
    powerRock,
    powerWater,
    powerFire,
    interact,
    aaaa,
    wahoo,
    moveYourBody,

    // Level
    doorUnlocked,
    doorLocked,
    lavaToStone,
    keyCollected,
    keySpawned,
    pressurePlateOn,
    pressurePlateOff,
    crystalHealed,
    checkKeyID,
    unlocking,
    crystalCollected,

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

    // General
    vulnerableStart,
    vulnerableEnd,
    gameOver,
    snapSound,
    switchSound,
    chainSnap,

    // Cobra boss
    cobraBarrageFire,
    cobraDamaged,
    cobraDeath,
    cobraMirageDamaged,
    cobraMirageWall,
    cobraPotBreak,
    cobraPotFire,
    cobraPotThud,
    cobraPotBigThud,

    // Added stuff
    fireBugKilled = 400,
    grubKilled,

    // Music
    menuMusic = 800,
    wMusic,
    stopMusic,
    wToadMusic,
    fMusic,
    fCentipedeMusic = 805,
    dMusic,
    dCobraMusic,
    oceanMan,
    overworldMusic
}

[System.Serializable]
public class MessageBusEvent : UnityEvent<string>
{

}

public static class MessageBus
{
    private static Dictionary<EMessageType, UnityEvent<string>> s_eventDict = new Dictionary<EMessageType, UnityEvent<string>>();

    private static int s_check = 0;

    // Subscribes a listener to a type of message
    public static void AddListener(EMessageType _type, UnityAction<string> _listener)
    {
        if (_type == EMessageType.none) { return; }

        // Event already exists
        if (s_eventDict.TryGetValue(_type, out UnityEvent<string> checkEvent))
        {
            checkEvent.AddListener(_listener);
        }
        // Event doesn't exist, create
        else
        {
            UnityEvent<string> newEvent = new MessageBusEvent();
            
            newEvent.AddListener(_listener);
            s_eventDict.Add(_type, newEvent);
        }
    }

    public static void RemoveListener(EMessageType _type, UnityAction<string> _listener)
    {
        if (_type == EMessageType.none) { return; }

        UnityEvent<string> checkEvent = null;

        // Only try to remove listener if event exists
        if (s_eventDict.TryGetValue(_type, out checkEvent))
        {
            checkEvent.RemoveListener(_listener);
        }
    }

    public static void TriggerEvent(EMessageType _type, string _param)
    {
        if (_type == EMessageType.none) { return; }

        UnityEvent<string> triggerEvent = null;

        if (s_eventDict.TryGetValue(_type, out triggerEvent))
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

        if (s_eventDict.TryGetValue(_type, out triggerEvent))
        {
            triggerEvent.Invoke(_type.ToString());
        }
    }
}