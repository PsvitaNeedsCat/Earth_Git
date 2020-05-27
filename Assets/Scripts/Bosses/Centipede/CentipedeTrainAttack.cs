using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeTrainAttack : CentipedeBehaviour
{
    public override void StartBehaviour()
    {
        base.StartBehaviour();
        Debug.Log("Train started");
        StartCoroutine(CompleteAfterSeconds(3.0f));
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
        Debug.Log("Train finished");
    }

    public override void Reset()
    {
        base.Reset();
    }
}
