﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System.Linq;
using UnityEditor;

public class CobraShuffle : CobraBehaviour
{
    public GameObject m_cobraMesh;
    public CobraEyes m_bossPotEyes;

    public static int s_bossPotIndex = 2;
    private static int s_nextBossPotIndex = 2;

    private List<CobraMoveDef> m_cobraMoves = new List<CobraMoveDef>();
    private List<CobraShufflePotDef> m_activePotDefs = new List<CobraShufflePotDef>();
    private List<CobraPot> m_activePots = new List<CobraPot>();

    // Start positions and orientations of all pots
    public static List<Vector3> s_potStartingPositions = new List<Vector3>();
    public static List<Quaternion> s_potStartingRotations = new List<Quaternion>();

    public AnimationCurve m_risingVerticalCurve;
    public AnimationCurve m_fallingVerticalCurve;
    public AnimationCurve m_horizontalMovementCurve;

    private readonly float m_lowJumpHeight = 1.5f;
    private readonly float m_mediumJumpHeight = 3.5f;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < s_boss.m_cobraPots.Count; i++)
        {
            s_potStartingPositions.Add(s_boss.m_cobraPots[i].GetMoveTransform().position);
            s_potStartingRotations.Add(s_boss.m_cobraPots[i].GetMoveTransform().rotation);
            s_boss.m_cobraPots[i].m_potIndex = i;
        }
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        GetPots();

        // Generate an order of moves
        GenerateMoves();

        // Fade in mirage cobras
        StartCoroutine(FadeInCobras());
    }

    private void GetPots()
    {
        m_activePots.Clear();

        m_activePotDefs = CobraHealth.StateSettings.m_shufflePotsToJumpIn;

        for (int i = 0; i < m_activePotDefs.Count; i++)
        {
            // CobraPot 
            m_activePots.Add(s_boss.m_cobraPots[m_activePotDefs[i].m_potIndex]);
            // s_boss.m_cobraPots[m_activePotDefs[i].m_potIndex].SetColliderDamage(true);
        }
    }

    private IEnumerator FadeInCobras()
    {
        foreach (CobraPot pot in m_activePots)
        {
            CobraMirageSpit spit = pot.GetComponent<CobraMirageSpit>();

            if (!spit.m_isReal)
            {
                spit.ExitPotFade();
                EffectsManager.SpawnEffect(EffectsManager.EEffectType.mirageSummon, spit.transform.position + Vector3.up * 0.75f, spit.transform.rotation, Vector3.one, 5.0f);
            }
        }

        yield return new WaitForSeconds(3.0f);

        foreach(CobraPot pot in m_activePots)
        {
            pot.GetComponent<CobraMirageSpit>().EnterPot();
        }

        yield return new WaitForSeconds(2.0f);

        m_bossPotEyes.gameObject.SetActive(true);
        m_bossPotEyes.Fade(true, 0.5f);

        // Jump into the middle
        StartCoroutine(JumpIn());
    }

    // Pots jump into the center of the arena
    private IEnumerator JumpIn()
    {
        // Start delay
        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleStartDelay);

        // Indices of pots jumping in
        List<int> potIndices = new List<int>();

        // Pots jump in
        for (int i = 0; i < m_activePots.Count; i++)
        {
            // Store indices of pots jumping in
            CobraPot thisPot = m_activePots[i];
            potIndices.Add(thisPot.m_potIndex);

            // This pot jumps in
            Vector3 jumpInPos = CobraMovementGrid.WorldPosFromIndex(m_activePotDefs[i].m_jumpInPoint);
            MovePot(thisPot, jumpInPos - thisPot.GetMoveTransform().position, 2.0f, CobraHealth.StateSettings.m_shuffleJumpInTime, true, m_fallingVerticalCurve);
        }

        bool bossMoved = false;

        // Generate final positions for the pots
        RandomShuffle(ref potIndices);

        for (int i = 0; i < m_activePots.Count; i++)
        {
            CobraPot thisPot = m_activePots[i];
            int newIndex = potIndices[i];
            thisPot.m_endIndex = newIndex;
            thisPot.m_endRotation = s_potStartingRotations[newIndex];

            if (!bossMoved && thisPot.m_potIndex == s_bossPotIndex)
            {
                s_bossPotIndex = newIndex;
            }
        }

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleJumpInTime + CobraHealth.StateSettings.m_shuffleIndicatorShowTime + CobraHealth.StateSettings.m_shuffleMoveDelay);

        StartCoroutine(MoveSequence());
    }

    public static void RandomShuffle<T>(ref List<T> _target)
    {
        _target = _target.OrderBy(x => System.Guid.NewGuid()).ToList();
    }

    // Pots move around the arena
    private IEnumerator MoveSequence()
    {
        // Do move sequence
        for (int i = 0; i < m_cobraMoves.Count; i++)
        {
            if (i == 1)
            {
                m_bossPotEyes.Fade(false, 0.25f);
                yield return new WaitForSeconds(0.25f);
                m_bossPotEyes.gameObject.SetActive(false);
            }

            CobraMoveDef move = m_cobraMoves[i];
            float waitFor = ExecuteMove(move.m_actionType, move.m_moveType);
            yield return new WaitForSeconds(waitFor);

            MessageBus.TriggerEvent(EMessageType.cobraPotThud);

            yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleMoveDelay);
        }

        StartCoroutine(JumpOut());
    }

    // Pots jump back out of the arena
    private IEnumerator JumpOut()
    {
        // Jump out
        yield return null;

        // Do jumping out
        for (int i = 0; i < m_activePots.Count; i++)
        {
            m_activePots[i].JumpOut(CobraHealth.StateSettings.m_shuffleJumpOutTime);

            yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleJumpOutDelay);
        }

        s_nextBossPotIndex = s_boss.m_cobraPots[s_bossPotIndex].m_endIndex;

        for (int i = 0; i < m_activePots.Count; i++)
        {
            m_activePots[i].m_potIndex = m_activePots[i].m_endIndex;
        }

        // Reorder the list of pots
        s_boss.SortPotList();

        s_bossPotIndex = s_nextBossPotIndex;

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleJumpOutTime);

        //for (int i = 0; i < m_activePots.Count; i++)
        //{
        //    m_activePots[i].SetColliderDamage(true);
        //}
        // Complete behaviour
        CompleteBehaviour();
    }

    // Returns how long the attack will take
    private float ExecuteMove(EShuffleActionType _actionType, EShuffleMoveType _moveType)
    {
        // Debug.Log("Executing move " + _actionType + _moveType);

        if (_actionType == EShuffleActionType.inOrOut)
        {
            return ExpandContract();
        }
        else
        {
            switch (_moveType)
            {
                case EShuffleMoveType.rotate:
                    {
                        return RotatePots(true);
                    }

                case EShuffleMoveType.swap:
                    {
                        return SwapPots();
                    }

                case EShuffleMoveType.fakeOut:
                    {
                        return FakeOutPots();
                    }

                case EShuffleMoveType.sideToSide:
                    {
                        return SideToSide();
                    }

                case EShuffleMoveType.complexRotate:
                    {
                        return ComplexRotate();
                    }

                default: return 1.0f;
            }
        }
    }

    // Generates a list of moves for the pots to do
    private void GenerateMoves()
    {
        m_cobraMoves.Clear();
        CobraStateSettings stateSettings = CobraHealth.StateSettings;

        for (int i = 0; i < stateSettings.m_shuffleNumMoves; i++)
        {
            EShuffleActionType actionType = (i % 3 == 0) ? EShuffleActionType.inOrOut : EShuffleActionType.move;
            EShuffleMoveType moveType = GetRandomMove();

            CobraMoveDef moveDef = new CobraMoveDef(actionType, moveType);
            m_cobraMoves.Add(moveDef);
        }
    }

    private EShuffleMoveType GetRandomMove()
    {
        int max = CobraHealth.StateSettings.m_allowedMoveTypes.Count;

        return (EShuffleMoveType)Random.Range(0, max);
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
    }

    public override void Reset()
    {
        base.Reset();
    }

    private void MovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles, AnimationCurve _easeCurve)
    {
        StartCoroutine(StartMovePot(_pot, _moveBy, _jumpHeight, _duration, _fireProjectiles, _easeCurve));
    }

    private IEnumerator StartMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles, AnimationCurve _easeCurve)
    {
        _pot.EnablePotIndicator(_pot.m_moveTransform.position + _moveBy);

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleIndicatorShowTime);

        // _pot.GetMoveTransform().DOBlendableMoveBy(_moveBy, _duration).SetEase(m_horizontalMovementCurve);
        // _pot.m_mesh.transform.DOPunchPosition(Vector3.up * _jumpHeight, _duration, 0, 0).SetEase(_easeCurve);
        _pot.GetMoveTransform().DOBlendableMoveBy(_moveBy + Vector3.up * _jumpHeight, _duration / 2.0f).SetEase(m_horizontalMovementCurve);

        yield return new WaitForSeconds(_duration / 2.0f);

        _pot.SetColliderDamage(true);

        _pot.GetMoveTransform().DOBlendableMoveBy(-Vector3.up * _jumpHeight, _duration / 2.0f).SetEase(m_fallingVerticalCurve);

        yield return new WaitForSeconds(_duration / 2.0f);

        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);
        Vector3 effectPosition = _pot.m_potLandIndicator.transform.position;
        EffectsManager.SpawnEffect(EffectsManager.EEffectType.cobraPotLand, effectPosition, Quaternion.identity, Vector3.one, 1.0f);

        _pot.SetColliderDamage(false);

        if (_fireProjectiles)
        {
            _pot.FireLobProjectiles();

            if (_pot.gameObject == gameObject)
            {
                MessageBus.TriggerEvent(EMessageType.cobraPotFire);
            }
        }

        _pot.DisablePotIndicator();
    }

    private void FakeMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles, AnimationCurve _easeCurve)
    {
        StartCoroutine(StartFakeMovePot(_pot, _moveBy, _jumpHeight, _duration, _fireProjectiles, _easeCurve));
    }

    private IEnumerator StartFakeMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles, AnimationCurve _easeCurve)
    {
        _pot.EnablePotIndicator(_pot.m_moveTransform.position);

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleIndicatorShowTime);

        // _pot.GetMoveTransform().DOPunchPosition(_moveBy, _duration, 0, 0).SetEase(m_horizontalMovementCurve);
        // _pot.m_mesh.transform.DOPunchPosition(Vector3.up * _jumpHeight, _duration, 0, 0).SetEase(_easeCurve);

        _pot.GetMoveTransform().DOPunchPosition(_moveBy, _duration, 0, 0).SetEase(m_horizontalMovementCurve);
        _pot.m_mesh.transform.DOBlendableMoveBy(Vector3.up * _jumpHeight, _duration / 2.0f).SetEase(m_risingVerticalCurve);

        yield return new WaitForSeconds(_duration / 2.0f);

        _pot.SetColliderDamage(true);

        _pot.m_mesh.transform.DOBlendableMoveBy(-Vector3.up * _jumpHeight, _duration / 2.0f).SetEase(_easeCurve);

        yield return new WaitForSeconds(_duration / 2.0f);

        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);
        _pot.SetColliderDamage(false);

        if (_fireProjectiles)
        {
            _pot.FireLobProjectiles();

            if (_pot.gameObject == gameObject)
            {
                MessageBus.TriggerEvent(EMessageType.cobraPotFire);
            }
        }

        _pot.DisablePotIndicator();
    }
    
    private float ExpandContract()
    {
        for (int i = 0; i < m_activePots.Count; i++)
        {
            int potTileIndex = CobraMovementGrid.IndexFromWorldPos(m_activePots[i].GetMoveTransform().position);

            if (potTileIndex >= 0)
            {
                Vector3 moveDir = CobraBoss.s_settings.m_expandContractDirections[potTileIndex];
                MovePot(m_activePots[i], moveDir, m_lowJumpHeight, CobraHealth.StateSettings.m_shuffleContractTime, true, m_fallingVerticalCurve);
            }
            else
            {
                Debug.LogError("Couldn't find what tile the pot was on");
            }
        }

        return CobraHealth.StateSettings.m_shuffleContractTime + CobraHealth.StateSettings.m_shuffleIndicatorShowTime;
    }

    private void RotatePot(CobraPot _pot, bool _clockwise)
    {
        int potTileIndex = CobraMovementGrid.IndexFromWorldPos(_pot.GetMoveTransform().position);
        Vector3 moveDir = CobraBoss.s_settings.m_rotateClockwiseDirections[potTileIndex];

        if (!_clockwise)
        {
            moveDir = -moveDir;
        }

        MovePot(_pot, moveDir, m_lowJumpHeight, CobraHealth.StateSettings.m_shuffleRotateJumpTime, true, m_fallingVerticalCurve);
    }

    private float RotatePots(bool _clockwise)
    {
        for (int i = 0; i < m_activePots.Count; i++)
        {
            CobraPot pot = m_activePots[i];
            RotatePot(pot, _clockwise);
        }

        return CobraHealth.StateSettings.m_shuffleRotateJumpTime + CobraHealth.StateSettings.m_shuffleIndicatorShowTime;
    }

    private void SwapPair(CobraPot _potOne, CobraPot _potTwo)
    {
        Vector3 moveVec = _potTwo.GetMoveTransform().position - _potOne.GetMoveTransform().position;
        moveVec.y = 0.0f;

        MovePot(_potOne, moveVec, m_mediumJumpHeight, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true, m_fallingVerticalCurve);
        MovePot(_potTwo, -moveVec, m_lowJumpHeight, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true, m_fallingVerticalCurve);
    }

    private float SwapPots()
    {
        for (int i = 0; i < m_activePots.Count / 2; i++)
        {
            SwapPair(m_activePots[2 * i], m_activePots[2 * i + 1]);
        }

        return CobraHealth.StateSettings.m_shuffleSwapJumpTime + CobraHealth.StateSettings.m_shuffleIndicatorShowTime;
    }

    private void FakeOutPair(CobraPot _potOne, CobraPot _potTwo)
    {
        Vector3 moveVec = _potTwo.GetMoveTransform().position - _potOne.GetMoveTransform().position;
        moveVec.y = 0.0f;

        moveVec /= 2.0f;

        FakeMovePot(_potOne, moveVec, m_mediumJumpHeight, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true, m_fallingVerticalCurve);
        FakeMovePot(_potTwo, -moveVec, m_lowJumpHeight, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true, m_fallingVerticalCurve);
    }

    private float FakeOutPots()
    {
        for (int i = 0; i < m_activePots.Count / 2; i++)
        {
            FakeOutPair(m_activePots[2 * i], m_activePots[2 * i + 1]);
        }

        return CobraHealth.StateSettings.m_shuffleSwapJumpTime + CobraHealth.StateSettings.m_shuffleIndicatorShowTime;
    }

    private float SideToSide()
    {
        for (int i = 0; i < m_activePots.Count; i++)
        {
            CobraPot pot = m_activePots[i];
            int potTileIndex = CobraMovementGrid.IndexFromWorldPos(pot.GetMoveTransform().position);
            Vector3 moveDir = CobraBoss.s_settings.m_sideToSideDirections[potTileIndex];

            MovePot(m_activePots[i], moveDir, m_lowJumpHeight, CobraHealth.StateSettings.m_shuffleSideToSideJumpTime, true, m_fallingVerticalCurve);
        }

        return CobraHealth.StateSettings.m_shuffleSideToSideJumpTime + CobraHealth.StateSettings.m_shuffleIndicatorShowTime;
    }

    private float ComplexRotate()
    {
        float jumpTime = CobraHealth.StateSettings.m_shuffleComplexRotateJumpTime;

        if (m_activePots.Count == 4)
        {
            Vector3 oneMove = m_activePots[3].GetMoveTransform().position - m_activePots[0].GetMoveTransform().position;
            Vector3 twoMove = m_activePots[0].GetMoveTransform().position - m_activePots[1].GetMoveTransform().position;
            Vector3 threeMove = (m_activePots[3].GetMoveTransform().position - m_activePots[2].GetMoveTransform().position) / 2.0f;
            Vector3 fourMove = m_activePots[1].GetMoveTransform().position - m_activePots[3].GetMoveTransform().position;

            MovePot(m_activePots[0], oneMove, m_lowJumpHeight, jumpTime, true, m_fallingVerticalCurve);
            MovePot(m_activePots[1], twoMove, m_lowJumpHeight, jumpTime, true, m_fallingVerticalCurve);
            FakeMovePot(m_activePots[2], threeMove, m_lowJumpHeight, jumpTime, true, m_fallingVerticalCurve);
            MovePot(m_activePots[3], fourMove, m_mediumJumpHeight, jumpTime, true, m_fallingVerticalCurve);
        }
        else
        {
            Vector3 oneMove = m_activePots[1].GetMoveTransform().position - m_activePots[0].GetMoveTransform().position;
            Vector3 twoMove = m_activePots[3].GetMoveTransform().position - m_activePots[1].GetMoveTransform().position;
            Vector3 threeMove = m_activePots[5].GetMoveTransform().position - m_activePots[2].GetMoveTransform().position;
            Vector3 fourMove = m_activePots[0].GetMoveTransform().position - m_activePots[3].GetMoveTransform().position;
            Vector3 fiveMove = m_activePots[2].GetMoveTransform().position - m_activePots[4].GetMoveTransform().position;
            Vector3 sixMove = m_activePots[4].GetMoveTransform().position - m_activePots[5].GetMoveTransform().position;

            MovePot(m_activePots[0], oneMove, m_lowJumpHeight, jumpTime, true, m_fallingVerticalCurve);
            MovePot(m_activePots[1], twoMove, m_lowJumpHeight, jumpTime, true, m_fallingVerticalCurve);
            MovePot(m_activePots[2], threeMove, m_lowJumpHeight, jumpTime, true, m_fallingVerticalCurve);
            MovePot(m_activePots[3], fourMove, m_mediumJumpHeight, jumpTime, true, m_fallingVerticalCurve);
            MovePot(m_activePots[4], fiveMove, m_mediumJumpHeight, jumpTime, true, m_fallingVerticalCurve);
            MovePot(m_activePots[5], sixMove, m_mediumJumpHeight, jumpTime, true, m_fallingVerticalCurve);
        }

        return jumpTime + CobraHealth.StateSettings.m_shuffleIndicatorShowTime;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (s_potStartingPositions.Count <= 0)
        {
            return;
        }

        if (s_boss == null)
        {
            return;
        }

        for (int i = 0; i < s_boss.m_cobraPots.Count; i++)
        {
            Handles.Label(s_potStartingPositions[i] + Vector3.up * 1.5f, s_boss.m_cobraPots[i].m_potIndex.ToString());
        }

        Handles.Label(transform.position + Vector3.up * 5.0f, "Boss pot index: " + s_bossPotIndex.ToString());
#endif
    }
}
