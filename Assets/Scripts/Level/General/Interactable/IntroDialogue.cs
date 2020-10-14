using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogue : Dialogue
{
    private bool m_timing = false;
    private Coroutine m_timingCorutine = null;


    public override void OnDisable()
    {
        StopAllCoroutines();

        base.OnDisable();
    }

    public override void Update()
    {
        base.Update();

        if (!m_timing && m_charIndex >= m_curDialogue.Length)
        {
            m_timing = true;
            m_timingCorutine = StartCoroutine(AutoplayDialogue(2.0f));
        }
    }

    private IEnumerator AutoplayDialogue(float _afterSeconds)
    {
        yield return new WaitForSeconds(_afterSeconds);

        base.ContinueDialogue("");

        m_timing = false;
    }

    public override void ContinueDialogue(string _null)
    {
        m_timing = false;
        if (m_timingCorutine != null)
        {
            StopCoroutine(m_timingCorutine);
        }

        base.ContinueDialogue(_null);
    }
}
