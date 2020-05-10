using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadIdle : ToadBehaviour
{
    public float m_idleDuration = 3.0f;

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        StartCoroutine(CompleteAfterSeconds(m_idleDuration));
    }
}
