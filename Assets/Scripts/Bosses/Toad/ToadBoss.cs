using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBT;

public class ToadBoss : MonoBehaviour
{
    public bool finished = false;

    #region Composite Nodes
    BaseNode attackSeq;
    BaseNode tongueSeq;
    BaseNode spitSwellSelector;
    BaseNode swellSeq;
    BaseNode waveSeq;
    #endregion

    #region Behaviour Nodes
    // Nodes
    BaseNode idleOne;
    BaseNode idleTwo;
    BaseNode idleThree;

    BaseNode tongueWindUp;
    BaseNode tongueExtend;
    BaseNode tongueRetract;
    BaseNode tongueWindDown;

    BaseNode spit;

    BaseNode swellUp;
    BaseNode swellStay;
    BaseNode swellDown;

    BaseNode waveJumpOff;
    BaseNode waveWait;
    BaseNode waveJumpOn;
    #endregion

    private void Awake()
    {
        idleOne = new LogNode(3.0f, "Idle one started", "Idle one finished");
        idleTwo = new LogNode(3.0f, "Idle two started", "Idle two finished");
        idleThree = new LogNode(3.0f, "Idle three started", "Idle three finished");

        tongueWindUp = new LogNode(2.0f, "Tongue windup started", "Tongue windup finished");
        tongueExtend = new LogNode(0.5f, "Tongue extend started", "Tongue extend finished");
        tongueRetract = new LogNode(0.5f, "Tongue retract started", "Tongue retract finished");
        tongueWindDown = new LogNode(2.0f, "Tongue winddown started", "Tongue winddown finished");

        spit = new LogNode(5.0f, "Spit attack started", "Spit attack finished");

        swellUp = new LogNode(1.0f, "Swell up started", "Swell up finished");
        swellStay = new LogNode(4.0f, "Swell stay started", "Swell stay finished");
        swellDown = new LogNode(1.0f, "Swell down started", "Swell down finished");

        waveJumpOff = new LogNode(3.0f, "Wave jump off started", "Wave jump off finished");
        waveWait = new LogNode(4.0f, "Wave wait started", "Wave wait finished");
        waveJumpOn = new LogNode(3.0f, "Wave jump on started", "Wave jump on finished");

        tongueSeq = new Sequence(new List<BaseNode>
        {
            tongueWindUp,
            tongueExtend,
            tongueRetract,
            tongueWindDown
        });

        swellSeq = new Sequence(new List<BaseNode>
        {
            swellUp,
            swellStay,
            swellDown
        });

        spitSwellSelector = new Selector(new List<BaseNode>
        {
            //spit,
            swellSeq,
            spit
        });

        waveSeq = new Sequence(new List<BaseNode>
        {
            waveJumpOff,
            waveWait,
            waveJumpOn
        });

        attackSeq = new Sequence(new List<BaseNode>
        {
            idleOne,
            tongueSeq,
            idleTwo,
            spitSwellSelector,
            idleThree,
            waveSeq
        });
    }

    private void Update()
    {
        if (finished) return;

        ENodeState result = attackSeq.Process();

        if (result == ENodeState.Success) finished = true;
    }

    #region Idle
    void Idle()
    {

    }
    #endregion

    #region Tongue Attack
    void TongueWindUp()
    {

    }

    void TongueExtend()
    {

    }

    void TongueRetract()
    {

    }

    void TongueWindDown()
    {

    }
    #endregion

    #region Spit Attack
    void Spit()
    {

    }
    #endregion

    #region Swell Up
    void SwellUp()
    {

    }

    void SwellStay()
    {

    }

    void SwellDown()
    {

    }
    #endregion

    #region Wave Attack
    void WaveJumpOff()
    {

    }

    void WaveWait()
    {

    }

    void WaveJumpOn()
    {

    }
    #endregion
}
