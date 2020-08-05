using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System.Linq;
using UnityEditor;

public class CobraShuffle : CobraBehaviour
{
    public List<CobraPot> m_pots;
    public GameObject m_cobraMesh;

    public static int s_bossPotIndex = 2;
    private static int s_nextBossPotIndex = 2;

    private List<CobraMoveDef> m_cobraMoves = new List<CobraMoveDef>();
    private List<CobraShufflePotDef> m_activePotDefs = new List<CobraShufflePotDef>();
    private List<CobraPot> m_activePots = new List<CobraPot>();

    // Start positions and orientations of all pots
    public static List<Vector3> s_potStartingPositions = new List<Vector3>();
    public static List<Quaternion> s_potStartingRotations = new List<Quaternion>();

    private List<int> m_toShuffle = new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private readonly Ease m_verticalEaseType = Ease.InOutSine;
    private readonly Ease m_horizontalEaseType = Ease.Linear;

    private void Awake()
    {
        for (int i = 0; i < m_pots.Count; i++)
        {
            s_potStartingPositions.Add(m_pots[i].transform.position);
            s_potStartingRotations.Add(m_pots[i].transform.rotation);
            m_pots[i].m_potIndex = i;
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
            m_pots[m_activePotDefs[i].m_potIndex].SetCollider(true);
        }
    }

    // Pots jump into the center of the arena
    private IEnumerator JumpIn()
    {
        m_cobraMesh.SetActive(false);

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
            MovePot(thisPot, jumpInPos - thisPot.transform.position, 2.0f, CobraHealth.StateSettings.m_shuffleJumpInTime, true, m_verticalEaseType);
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

        //// Store the indices of the pots that jump in
        //List<int> potStartIndices = new List<int>();
        //List<Quaternion> potStartOrientations = new List<Quaternion>();

        //// Do jumping in
        //for (int i = 0; i < m_activePotDefs.Count; i++)
        //{
        //    // Add the indices and orientations of pots that are jumping in to the lists
        //    potStartIndices.Add(m_activePotDefs[i].m_potIndex);
        //    potStartOrientations.Add(m_activePots[i].transform.rotation);

        //    Vector3 jumpInPos = CobraMovementGrid.WorldPosFromIndex(m_activePotDefs[i].m_jumpInPoint);
        //    MovePot(m_activePots[i], jumpInPos - m_activePots[i].transform.position, 2.0f, CobraHealth.StateSettings.m_shuffleJumpInTime, true);
        //}

        //string potIndicesString = "";
        //foreach(int index in potStartIndices)
        //{
        //    potIndicesString += index.ToString() + " ";
        //}
        //Debug.Log("Pot indices: " + potIndicesString);

        //bool bossMoved = false;

        //// Generate final positions for the pots
        //for (int j = 0; j < m_activePots.Count; j++)
        //{
        //    int randomIndexPosition = Random.Range(0, potStartIndices.Count);
        //    int randomIndex = potStartIndices[randomIndexPosition];

        //    Debug.Log("Pot " + j + " final position is now " + randomIndex);

        //    m_activePots[j].m_finalPosition = s_potStartingPositions[randomIndex];
        //    m_activePots[j].m_finalOrientation = potStartOrientations[randomIndexPosition];
        //    m_activePots[j].m_finalIndex = randomIndex;

        //    if (!bossMoved && m_activePots[j].m_isBoss)
        //    {
        //        bossMoved = true;
        //        Debug.Log("Boss was at index " + s_bossPotIndex + ", now moving to index " + potStartIndices[randomIndexPosition]);
        //        SetBossPot(potStartIndices[randomIndexPosition]);
        //    }

        //    potStartIndices.RemoveAt(randomIndexPosition);
        //    potStartOrientations.RemoveAt(randomIndexPosition);
        //}

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleJumpInTime);

        StartCoroutine(MoveSequence());
    }

    public static void RandomShuffle<T>(ref List<T> _target)
    {
        _target = _target.OrderBy(x => System.Guid.NewGuid()).ToList();
    }

    //public static void PrintList<T>(string _first, List<T> _target)
    //{
    //    string s = "";
    //    foreach(T item in _target)
    //    {
    //        s += item.ToString();
    //    }

    //    Debug.Log(_first + s);
    //}

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
            
            m_activePots[i].SetCollider(false);
        }

        s_nextBossPotIndex = m_pots[s_bossPotIndex].m_endIndex;

        for (int i = 0; i < m_activePots.Count; i++)
        {
            m_activePots[i].m_potIndex = m_activePots[i].m_endIndex;
        }

        // Reorder the list of pots
        m_pots.Sort((pOne, pTwo) => pOne.m_potIndex.CompareTo(pTwo.m_potIndex));

        for (int i = 0; i < m_pots.Count; i++)
        {
            m_pots[i].m_potIndex = i;
        }

        s_bossPotIndex = s_nextBossPotIndex;

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleJumpOutTime);

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

    private void MovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles, Ease _easeType)
    {
        StartCoroutine(StartMovePot(_pot, _moveBy, _jumpHeight, _duration, _fireProjectiles, _easeType));
    }

    private IEnumerator StartMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles, Ease _easeType)
    {
        _pot.transform.DOBlendableMoveBy(_moveBy, _duration).SetEase(m_horizontalEaseType);
        _pot.m_mesh.transform.DOPunchPosition(Vector3.up * _jumpHeight, _duration, 0, 0).SetEase(_easeType);

        if (_fireProjectiles)
        {
            yield return new WaitForSeconds(_duration);
            _pot.FireLobProjectiles();
            MessageBus.TriggerEvent(EMessageType.cobraPotFire);
        }
    }

    private void FakeMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles, Ease _easeType)
    {
        StartCoroutine(StartFakeMovePot(_pot, _moveBy, _jumpHeight, _duration, _fireProjectiles, _easeType));
    }

    private IEnumerator StartFakeMovePot(CobraPot _pot, Vector3 _moveBy, float _jumpHeight, float _duration, bool _fireProjectiles, Ease _easeType)
    {
        _pot.transform.DOPunchPosition(_moveBy, _duration, 0, 0).SetEase(m_horizontalEaseType);
        _pot.m_mesh.transform.DOPunchPosition(Vector3.up * _jumpHeight, _duration, 0, 0).SetEase(_easeType);

        if (_fireProjectiles)
        {
            yield return new WaitForSeconds(_duration);
            _pot.FireLobProjectiles();
            MessageBus.TriggerEvent(EMessageType.cobraPotFire);
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
                MovePot(m_activePots[i], moveDir, 1.0f, CobraHealth.StateSettings.m_shuffleContractTime, true, m_verticalEaseType);
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

        MovePot(_pot, moveDir, 1.0f, CobraHealth.StateSettings.m_shuffleRotateJumpTime, true, m_verticalEaseType);
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

        MovePot(_potOne, moveVec, 3.0f, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true, m_verticalEaseType);
        MovePot(_potTwo, -moveVec, 1.0f, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true, m_verticalEaseType);
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

        FakeMovePot(_potOne, moveVec, 3.0f, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true, m_verticalEaseType);
        FakeMovePot(_potTwo, -moveVec, 1.0f, CobraHealth.StateSettings.m_shuffleSwapJumpTime, true, m_verticalEaseType);
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

            MovePot(m_activePots[i], moveDir, 1.0f, CobraHealth.StateSettings.m_shuffleSideToSideJumpTime, true, m_verticalEaseType);
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

            MovePot(m_activePots[0], oneMove, 1.0f, jumpTime, true, m_verticalEaseType);
            MovePot(m_activePots[1], twoMove, 1.0f, jumpTime, true, m_verticalEaseType);
            FakeMovePot(m_activePots[2], threeMove, 1.0f, jumpTime, true, m_verticalEaseType);
            MovePot(m_activePots[3], fourMove, 1.0f, jumpTime, true, m_verticalEaseType);
        }
        else
        {
            Vector3 oneMove = m_activePots[1].transform.position - m_activePots[0].transform.position;
            Vector3 twoMove = m_activePots[3].transform.position - m_activePots[1].transform.position;
            Vector3 threeMove = m_activePots[5].transform.position - m_activePots[2].transform.position;
            Vector3 fourMove = m_activePots[0].transform.position - m_activePots[3].transform.position;
            Vector3 fiveMove = m_activePots[2].transform.position - m_activePots[4].transform.position;
            Vector3 sixMove = m_activePots[4].transform.position - m_activePots[5].transform.position;

            MovePot(m_activePots[0], oneMove, 1.0f, jumpTime, true, m_verticalEaseType);
            MovePot(m_activePots[1], twoMove, 1.0f, jumpTime, true, m_verticalEaseType);
            MovePot(m_activePots[2], threeMove, 1.0f, jumpTime, true, m_verticalEaseType);
            MovePot(m_activePots[3], fourMove, 1.0f, jumpTime, true, m_verticalEaseType);
            MovePot(m_activePots[4], fiveMove, 1.0f, jumpTime, true, m_verticalEaseType);
            MovePot(m_activePots[5], sixMove, 1.0f, jumpTime, true, m_verticalEaseType);
        }

        return jumpTime;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (s_potStartingPositions.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < m_pots.Count; i++)
        {
            Handles.Label(s_potStartingPositions[i] + Vector3.up * 1.5f, m_pots[i].m_potIndex.ToString());
        }

        Handles.Label(transform.position + Vector3.up * 5.0f, "Boss pot index: " + s_bossPotIndex.ToString());
#endif
    }
}
