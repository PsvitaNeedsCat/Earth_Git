using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System.Linq;

public class CobraShuffle : CobraBehaviour
{
    public List<CobraPot> m_pots;

    public static int s_bossPotIndex = 2;

    private List<CobraMoveDef> m_cobraMoves = new List<CobraMoveDef>();
    private List<CobraShufflePotDef> m_activePotDefs = new List<CobraShufflePotDef>();
    private List<CobraPot> m_activePots = new List<CobraPot>();
    private int m_currentMoveIndex = 0;

    public static List<Vector3> s_potStartingPositions = new List<Vector3>();
    public static List<Quaternion> s_potStartingOrientations= new List<Quaternion>();

    private void Awake()
    {
        for (int i = 0; i < m_pots.Count; i++)
        {
            s_potStartingPositions.Add(m_pots[i].transform.position);
            s_potStartingOrientations.Add(m_pots[i].transform.rotation);
        }
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        GetPots();

        // Generate an order of moves
        GenerateMoves();

        // Jump into the middle
        StartCoroutine(JumpIn());
    }

    private void GetPots()
    {
        m_activePots.Clear();

        m_activePotDefs = CobraHealth.StateSettings.m_shufflePotsToJumpIn;

        for (int i = 0; i < m_activePotDefs.Count; i++)
        {
            // CobraPot 
            m_activePots.Add(m_pots[m_activePotDefs[i].m_potIndex]);

        }
    }

    // Pots jump into the center of the arena
    private IEnumerator JumpIn()
    {
        // Start delay
        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleStartDelay);

        List<int> potStartIndices = new List<int>();
        List<Quaternion> potStartOrientations = new List<Quaternion>();

        // Do jumping in
        for (int i = 0; i < m_activePotDefs.Count; i++)
        {
            potStartIndices.Add(m_activePotDefs[i].m_potIndex);
            potStartOrientations.Add(m_activePots[i].transform.rotation);

            Vector3 jumpInPos = CobraMovementGrid.WorldPosFromIndex(m_activePotDefs[i].m_jumpInPoint);
            MovePot(m_activePots[i], jumpInPos - m_activePots[i].transform.position, 2.0f, CobraHealth.StateSettings.m_shuffleJumpInTime, true);
        }

        // Generate final positions for the pots
        for (int i = 0; i < m_activePots.Count; i++)
        {
            int randomIndexPosition = Random.Range(0, potStartIndices.Count);
            int randomIndex = potStartIndices[randomIndexPosition];
            m_activePots[i].m_finalPosition = s_potStartingPositions[randomIndex];
            m_activePots[i].m_finalOrientation = potStartOrientations[randomIndexPosition];
            m_activePots[i].m_finalIndex = potStartIndices[randomIndexPosition];

            if (m_activePotDefs[i].m_potIndex == s_bossPotIndex)
            {
                Debug.Log("Boss was at index " + s_bossPotIndex + ", now moving to index " + potStartIndices[randomIndexPosition]);
                s_bossPotIndex = potStartIndices[randomIndexPosition];
            }

            potStartIndices.RemoveAt(randomIndexPosition);
            potStartOrientations.RemoveAt(randomIndexPosition);
        }

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleJumpInTime);

        StartCoroutine(MoveSequence());
    }

    public static void RandomShuffle<T>(ref List<T> _target)
    {
        _target.OrderBy(x => System.Guid.NewGuid()).ToList();
    }

    // Pots move around the arena
    private IEnumerator MoveSequence()
    {
        // Do move sequence
        for (int i = 0; i < m_cobraMoves.Count; i++)
        {
            CobraMoveDef move = m_cobraMoves[i];
            float waitFor = ExecuteMove(move.m_actionType, move.m_moveType);
            yield return new WaitForSeconds(waitFor);
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

        // Reorder the list of pots
        m_pots.Sort((pOne, pTwo) => pOne.m_finalIndex.CompareTo(pTwo.m_finalIndex));

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleJumpOutTime);

        // Complete behaviour
        CompleteBehaviour();
    }

    // Returns how long the attack will take
    private float ExecuteMove(EShuffleActionType _actionType, EShuffleMoveType _moveType)
    {
        Debug.Log("Executing move " + _actionType + _moveType);

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
            EShuffleActionType actionType = (i % 4 == 0) ? EShuffleActionType.inOrOut : EShuffleActionType.move;
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

    private void MovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles)
    {
        StartCoroutine(StartMovePot(_pot, _moveBy, _jumpHeight, _duration, _fireProjectiles));
    }

    private IEnumerator StartMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles)
    {
        _pot.transform.DOBlendableMoveBy(_moveBy, _duration);
        _pot.m_mesh.transform.DOPunchPosition(Vector3.up * _jumpHeight, _duration, 0, 0);
        
        if (_fireProjectiles)
        {
            yield return new WaitForSeconds(_duration);
            _pot.FireLobProjectiles();
        }
    }

    private void FakeMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles)
    {
        StartCoroutine(StartFakeMovePot(_pot, _moveBy, _jumpHeight, _duration, _fireProjectiles));
    }

    private IEnumerator StartFakeMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles)
    {
        _pot.transform.DOPunchPosition(_moveBy, _duration, 0, 0);
        _pot.m_mesh.transform.DOPunchPosition(Vector3.up * _jumpHeight, _duration, 0, 0);

        if (_fireProjectiles)
        {
            yield return new WaitForSeconds(_duration);
            _pot.FireLobProjectiles();
        }
    }
    
    private float ExpandContract()
    {
        for (int i = 0; i < m_activePots.Count; i++)
        {
            int potTileIndex = CobraMovementGrid.IndexFromWorldPos(m_activePots[i].transform.position);

            if (potTileIndex >= 0)
            {
                Vector3 moveDir = CobraBoss.s_settings.m_expandContractDirections[potTileIndex];
                MovePot(m_activePots[i], moveDir, 1.0f, CobraHealth.StateSettings.m_shuffleContractTime, true);
            }
            else
            {
                Debug.LogError("Couldn't find what tile the pot was on");
            }
        }

        return CobraHealth.StateSettings.m_shuffleContractTime;
    }

    private void RotatePot(CobraPot _pot, bool _clockwise)
    {
        int potTileIndex = CobraMovementGrid.IndexFromWorldPos(_pot.transform.position);
        Vector3 moveDir = CobraBoss.s_settings.m_rotateClockwiseDirections[potTileIndex];

        if (!_clockwise)
        {
            moveDir = -moveDir;
        }

        MovePot(_pot, moveDir, 1.0f, CobraHealth.StateSettings.m_shuffleRotateJumpTime, true);
    }

    private float RotatePots(bool _clockwise)
    {
        for (int i = 0; i < m_activePots.Count; i++)
        {
            CobraPot pot = m_activePots[i];
            RotatePot(pot, _clockwise);
        }

        return CobraHealth.StateSettings.m_shuffleRotateJumpTime;
    }

    private void SwapPair(CobraPot _potOne, CobraPot _potTwo)
    {
        Vector3 moveVec = _potTwo.transform.position - _potOne.transform.position;
        moveVec.y = 0.0f;

        MovePot(_potOne, moveVec, 3.0f, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true);
        MovePot(_potTwo, -moveVec, 1.0f, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true);
    }

    private float SwapPots()
    {
        for (int i = 0; i < m_activePots.Count / 2; i++)
        {
            SwapPair(m_activePots[2 * i], m_activePots[2 * i + 1]);
        }

        return CobraHealth.StateSettings.m_shuffleSwapJumpTime;
    }

    private void FakeOutPair(CobraPot _potOne, CobraPot _potTwo)
    {
        Vector3 moveVec = _potTwo.transform.position - _potOne.transform.position;
        moveVec.y = 0.0f;

        moveVec /= 2.0f;

        FakeMovePot(_potOne, moveVec, 3.0f, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true);
        FakeMovePot(_potTwo, -moveVec, 1.0f, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true);
    }

    private float FakeOutPots()
    {
        for (int i = 0; i < m_activePots.Count / 2; i++)
        {
            FakeOutPair(m_activePots[2 * i], m_activePots[2 * i + 1]);
        }

        return CobraHealth.StateSettings.m_shuffleSwapJumpTime;
    }

    private float SideToSide()
    {
        for (int i = 0; i < m_activePots.Count; i++)
        {
            CobraPot pot = m_activePots[i];
            int potTileIndex = CobraMovementGrid.IndexFromWorldPos(pot.transform.position);
            Vector3 moveDir = CobraBoss.s_settings.m_sideToSideDirections[potTileIndex];

            MovePot(m_activePots[i], moveDir, 1.0f, CobraHealth.StateSettings.m_shuffleSideToSideJumpTime, true);
        }

        return CobraHealth.StateSettings.m_shuffleSideToSideJumpTime;
    }

    private float ComplexRotate()
    {
        float jumpTime = CobraHealth.StateSettings.m_shuffleComplexRotateJumpTime;

        if (m_activePots.Count == 4)
        {
            Vector3 oneMove = m_activePots[3].transform.position - m_activePots[0].transform.position;
            Vector3 twoMove = m_activePots[0].transform.position - m_activePots[1].transform.position;
            Vector3 threeMove = (m_activePots[3].transform.position - m_activePots[2].transform.position) / 2.0f;
            Vector3 fourMove = m_activePots[1].transform.position - m_activePots[3].transform.position;

            MovePot(m_activePots[0], oneMove, 1.0f, jumpTime, true);
            MovePot(m_activePots[1], twoMove, 1.0f, jumpTime, true);
            FakeMovePot(m_activePots[2], threeMove, 1.0f, jumpTime, true);
            MovePot(m_activePots[3], fourMove, 1.0f, jumpTime, true);
        }
        else
        {
            Vector3 oneMove = m_activePots[1].transform.position - m_activePots[0].transform.position;
            Vector3 twoMove = m_activePots[3].transform.position - m_activePots[1].transform.position;
            Vector3 threeMove = m_activePots[5].transform.position - m_activePots[2].transform.position;
            Vector3 fourMove = m_activePots[0].transform.position - m_activePots[3].transform.position;
            Vector3 fiveMove = m_activePots[2].transform.position - m_activePots[4].transform.position;
            Vector3 sixMove = m_activePots[4].transform.position - m_activePots[5].transform.position;

            MovePot(m_activePots[0], oneMove, 1.0f, jumpTime, true);
            MovePot(m_activePots[1], twoMove, 1.0f, jumpTime, true);
            MovePot(m_activePots[2], threeMove, 1.0f, jumpTime, true);
            MovePot(m_activePots[3], fourMove, 1.0f, jumpTime, true);
            MovePot(m_activePots[4], fiveMove, 1.0f, jumpTime, true);
            MovePot(m_activePots[5], sixMove, 1.0f, jumpTime, true);
        }

        return jumpTime;
    }
}
