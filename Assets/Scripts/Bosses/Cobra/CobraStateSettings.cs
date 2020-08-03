using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECobraMirageType { none, blue, red }

public enum EDirection { northEast, southEast, southWest, northWest }

public enum EShuffleMoveType { rotate, swap, fakeOut, complexRotate, sideToSide }

public enum EShuffleActionType { move, inOrOut }

[System.Serializable]
public struct CobraMoveDef
{
    public EShuffleActionType m_actionType;
    public EShuffleMoveType m_moveType;

    public CobraMoveDef(EShuffleActionType _actionType, EShuffleMoveType _moveType)
    {
        m_actionType = _actionType;
        m_moveType = _moveType;
    }
}

[System.Serializable]
public struct CobraShufflePotDef
{
    public int m_potIndex;
    public int m_jumpInPoint;
}

[System.Serializable]
public struct CobraMirageWallDef
{
    public ECobraMirageType m_wallOneType;
    public EDirection m_wallOneFrom; // Direction that the wall comes from
    public ECobraMirageType m_wallTwoType; // If set to none, no wall will be spawned
    public EDirection m_wallTwoFrom;
}

// Cobra boss settings that change based on health value
[CreateAssetMenu(fileName = "New Cobra State Settings", menuName = "Settings/CobraStateSettings")]
public class CobraStateSettings : ScriptableObject
{
    [Header("Mirage Wall Settings")]
    public CobraMirageWallDef[] m_wallSequence;
    public float m_timeBetweenWalls;
    public float m_wallDelayBeforeMove;
    public float m_wallMoveDuration;

    [Header("Sand Drop Settings")]
    public float m_potProjectileSpeed; // How fast the bullets of the pot should go
    public float m_numPotsToFire; // How many pots should fire during one attack sequence
    public float m_delayBetweenPots; // How much time will pass inbetween pots firing

    public int m_projectilesPerPot; // How many projectiles each pot will fire
    public float m_potProjectileInterval; // Time between firing projectiles by a pot
    public float m_potProjectileLifetime; // How long before the projectiles are despawned

    [Header("Shuffle Settings")]
    public List<EShuffleMoveType> m_allowedMoveTypes;
    public List<CobraShufflePotDef> m_shufflePotsToJumpIn;
    public float m_shuffleStartDelay;
    public float m_shuffleMoveDelay;
    public float m_shuffleJumpInTime;
    public float m_shuffleJumpOutTime;
    public float m_shuffleJumpOutDelay;
    public float m_shuffleContractTime;
    public int m_shuffleNumMoves;
    public float m_shuffleRotateJumpTime;
    public float m_shuffleSwapJumpTime;
    public float m_shuffleSideToSideJumpTime;
    public float m_shuffleComplexRotateJumpTime;

    [Header("Mirage Barrage Settings")]
    public List<int> m_barrageAttackPositions;

    public float m_barrageProjectileSpeed;
    public int m_barrageProjectilesPerHead;
    public float m_barrageProjectileInterval;
    public float m_barrageProjectileLifetime;
}
