﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// General settings for the cobra boss
[CreateAssetMenu(fileName = "New Cobra Global Settings", menuName = "Settings/CobraGlobalSettings")]
public class CobraGlobalSettings : ScriptableObject
{
    [Header("General Settings")]
    public int m_maxHealth = 3;
    public int m_arenaLength;
    public float m_sandDropHeight;

    [Header("Mirage Wall Settings")]
    public float m_wallTravelDistance;
    public float m_wallSpawnDistance;

    [Header("Mirage Block Scramble Settings")]
    public float m_timeBeforeGenerate;
    public List<string> m_blockLayouts; // B = Blue block, R = Red block, N = None

    [Header("Chase Settings")]
    public int m_jumpsBeforeDeath;
    public float m_jumpDuration;
    public float m_jumpDelay;
    public float m_bigJumpHeight;
    public float m_bigJumpDuration;
    public float m_chaseJumpHeight;
    public float m_chaseJumpRaiseTime;
    public float m_chaseJumpAimTime;
    public float m_chaseJumpFallTime;
    public float m_chaseSpeed;

    public AnimationCurve m_chaseRaiseCurve;
    public AnimationCurve m_chaseFallCurve;

    [Header("Jump Drop Shadow Settings")]
    public float m_dropShadowMinScale = 0.01f;
    public float m_dropShadowMaxScale = 0.1f;

    [Header("Shuffle Settings")]
    // When the cobra is at these positions, this is the direction it will go in order to expand / contract
    public Vector3[] m_expandContractDirections = 
    { 
        Vector3.right + -Vector3.forward,       -Vector3.forward,                       -Vector3.forward,              -Vector3.forward,                        -Vector3.right + -Vector3.forward,
        Vector3.right,                          -Vector3.right + Vector3.forward,        Vector3.forward,               Vector3.forward + Vector3.right,        -Vector3.right,
        Vector3.right,                          -Vector3.right,                          Vector3.zero,                  Vector3.right,                          -Vector3.right,
        Vector3.right,                          -Vector3.right + -Vector3.forward,      -Vector3.forward,               Vector3.right + -Vector3.forward,       -Vector3.right,
        Vector3.right + Vector3.forward,         Vector3.forward,                        Vector3.forward,               Vector3.forward,                        -Vector3.right + Vector3.forward
    };

    // When the cobra is at these positions, this is the direction it will go in order to rotate clockwise
    public Vector3[] m_rotateClockwiseDirections =
    {
        Vector3.right,    Vector3.right,    Vector3.right,  Vector3.right,   -Vector3.forward,
        Vector3.forward,  Vector3.right,    Vector3.right, -Vector3.forward, -Vector3.forward,
        Vector3.forward,  Vector3.forward,  Vector3.zero,  -Vector3.forward, -Vector3.forward,
        Vector3.forward,  Vector3.forward, -Vector3.right, -Vector3.right,   -Vector3.forward,
        Vector3.forward, -Vector3.right,   -Vector3.right, -Vector3.right,   -Vector3.right
    };

    // When the cobra is at these positions, this is the direction it will go in order to move side-to-side
    public Vector3[] m_sideToSideDirections =
    {
        Vector3.right, -Vector3.right, Vector3.zero, Vector3.right, -Vector3.right,
        Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero,
        Vector3.right, -Vector3.right, Vector3.zero, Vector3.right, -Vector3.right,
        Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero,
        Vector3.right, -Vector3.right, Vector3.zero, Vector3.right, -Vector3.right
    };
}
