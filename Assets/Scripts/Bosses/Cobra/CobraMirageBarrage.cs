using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageBarrage : CobraBehaviour
{
    public override void StartBehaviour()
    {
        base.StartBehaviour();

        StartCoroutine(CompleteAfterSeconds(3.0f));
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
