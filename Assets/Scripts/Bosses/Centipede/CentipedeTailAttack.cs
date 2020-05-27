using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeTailAttack : CentipedeBehaviour
{
    public override void StartBehaviour()
    {
        base.StartBehaviour();
        Debug.Log("Tail started");
        StartCoroutine(CompleteAfterSeconds(3.0f));
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
        Debug.Log("Tail finished");
    }

    public override void Reset()
    {
        base.Reset();
    }
}
