﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECobraMirageWallType { none, blue, red }
public enum EDirection { northEast, southEast, southWest, northWest }

[System.Serializable]
public struct CobraMirageWallDef
{
    public ECobraMirageWallType m_wallOneType;
    public EDirection m_wallOneFrom; // Direction that the wall comes from
    public ECobraMirageWallType m_wallTwoType; // If set to none, no wall will be spawned
    public EDirection m_wallTwoFrom;
}

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

    public int m_projectilesPerPot; // How many projectiles each pot will ifre
    public float m_potProjectileInterval; // Time between firing projectiles by a pot
    public float m_potProjectileLifetime; // How long before the projectiles are despawned
}
