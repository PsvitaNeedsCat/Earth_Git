using System.Collections;
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
    public CobraMirageWallDef[] m_wallSequence;
    
    [Header("Mirage Wall Settings")]
    public float m_timeBetweenWalls;
    public float m_wallDelayBeforeMove;
    public float m_wallMoveDuration;
}
