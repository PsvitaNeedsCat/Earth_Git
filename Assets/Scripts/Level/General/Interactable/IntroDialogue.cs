using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogue : Dialogue
{

    public override void OnEnable()
    {
        StartCoroutine(AutoplayDialogue(5.0f));
        StartCoroutine(AutoplayDialogue(8.0f));
        StartCoroutine(AutoplayDialogue(14.5f));
        StartCoroutine(AutoplayDialogue(21.0f));
        StartCoroutine(AutoplayDialogue(27.0f));
    }

    private IEnumerator AutoplayDialogue(float _afterSeconds)
    {
        yield return new WaitForSeconds(_afterSeconds);

        ContinueDialogue("");
    }

    public override void OnDisable()
    {
        StopAllCoroutines();
    }


}
