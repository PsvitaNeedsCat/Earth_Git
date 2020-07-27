using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraShuffle : CobraBehaviour
{
    public List<CobraPot> m_pots;

    private List<CobraMoveDef> m_cobraMoves;

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        

        // Generate an order of moves
        GenerateMoves();

        // Jump into the middle
        StartCoroutine(JumpIn());
    }

    // Pots jump into the center of the arena
    private IEnumerator JumpIn()
    {
        // Start delay
        yield return new WaitForSeconds(CobraHealth.StateSettings.m_shuffleStartDelay);

        // Do jumping in

        StartCoroutine(MoveSequence());
    }

    // Pots move around the arena
    private IEnumerator MoveSequence()
    {
        // Do move sequence
        yield return null;

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
}
