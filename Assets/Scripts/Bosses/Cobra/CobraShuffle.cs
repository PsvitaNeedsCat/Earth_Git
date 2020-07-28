using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CobraShuffle : CobraBehaviour
{
    public List<CobraPot> m_pots;

    private List<CobraMoveDef> m_cobraMoves = new List<CobraMoveDef>();
    private List<CobraShufflePotDef> m_activePotDefs = new List<CobraShufflePotDef>();
    private List<CobraPot> m_activePots = new List<CobraPot>();

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
            m_activePots.Add(m_pots[m_activePotDefs[i].m_potIndex]);
        }
    }

    // Pots jump into the center of the arena
    private IEnumerator JumpIn()
    {
        // Start delay
        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleStartDelay);

        // Do jumping in
        for (int i = 0; i < m_activePotDefs.Count; i++)
        {
            // Vector3 jumpInPos = CobraBoss.GetTileWorldPos(m_activePotDefs[i].m_jumpInPoint);
            Vector3 jumpInPos = CobraMovementGrid.WorldPosFromIndex(m_activePotDefs[i].m_jumpInPoint);
            MovePot(m_activePots[i], jumpInPos, 2.0f, CobraHealth.StateSettings.m_shuffleJumpInTime, true);
        }

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleJumpInTime);

        StartCoroutine(MoveSequence());
    }

    // Pots move around the arena
    private IEnumerator MoveSequence()
    {
        // Do move sequence
        yield return null;

        for (int i = 0; i < 10; i++)
        {
            ExpandContract();
            yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleContractTime * 2.0f);
        }

        StartCoroutine(JumpOut());
    }

    // Pots jump back out of the arena
    private IEnumerator JumpOut()
    {
        // Jump out
        yield return null;

        // Complete behaviour
        CompleteBehaviour();
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
        }
    }

    private EShuffleMoveType GetRandomMove()
    {
        return (EShuffleMoveType)Random.Range(0, 5);
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
    }

    public override void Reset()
    {
        base.Reset();
    }

    private void MovePot(CobraPot _pot, Vector3 _destination, float _jumpHeight, float _duration, bool _fireProjectiles)
    {
        StartCoroutine(StartMovePot(_pot, _destination, _jumpHeight, _duration, _fireProjectiles));
    }

    private IEnumerator StartMovePot(CobraPot _pot, Vector3 _destination, float _jumpHeight, float _duration, bool _fireProjectiles)
    {
        _pot.transform.DOMove(_destination, _duration);
        _pot.m_mesh.transform.DOPunchPosition(Vector3.up * _jumpHeight, _duration, 0, 0);
        
        if (_fireProjectiles)
        {
            yield return new WaitForSeconds(_duration);
            _pot.FireLobProjectiles();
        }
    }
    
    private void ExpandContract()
    {
        for (int i = 0; i < m_activePots.Count; i++)
        {
            int potTileIndex = CobraMovementGrid.IndexFromWorldPos(m_activePots[i].transform.position);

            if (potTileIndex >= 0)
            {
                Vector3 moveDir = -CobraBoss.s_settings.m_expandContractDirections[potTileIndex];
                Debug.Log("Moving pot at tile index: " + potTileIndex + " in direction: " + moveDir);
                MovePot(m_activePots[i], m_activePots[i].transform.position + moveDir, 1.0f, CobraHealth.StateSettings.m_shuffleContractTime, true);
            }
            else
            {
                Debug.LogError("Couldn't find what tile the pot was on");
            }
        }
    }
}
