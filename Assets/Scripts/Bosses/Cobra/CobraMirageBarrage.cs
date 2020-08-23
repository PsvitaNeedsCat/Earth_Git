using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cobra mirage barrage attack behaviour
public class CobraMirageBarrage : CobraBehaviour
{
    public GameObject m_cobraMesh;

    public static int s_shotsFired = 0;

    private List<CobraMirageSpit> m_mirageCobras = new List<CobraMirageSpit>();
    private CobraMirageSpit m_spit;
    private CobraBoss m_boss;
    private CobraShuffle m_shuffle;

    protected override void Awake()
    {
        base.Awake();

        // Initialise variables
        m_spit = GetComponent<CobraMirageSpit>();
        m_boss = GetComponent<CobraBoss>();
        m_shuffle = GetComponent<CobraShuffle>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        s_shotsFired = 0;

        StartCoroutine(StartSpawning());        
    }

    private IEnumerator StartSpawning()
    {
        // Store pots to fire
        GenerateSnakes();

        // Flip tiles and destroy chunks
        m_boss.FlipTiles();

        foreach (CobraMirageSpit spit in m_mirageCobras)
        {
            spit.CobraJump();
        }

        yield return new WaitForSeconds(0.2f);

        ChunkManager.DestroyAllChunks();

        yield return new WaitForSeconds(1.0f);

        
        ExitPots();

        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(FireProjectiles());
    }

    private void ExitPots()
    {
        // m_animations.ExitPot();

        foreach (CobraMirageSpit spit in m_mirageCobras)
        {
            spit.ExitPot();
        }
    }

    private void EnterPots()
    {
        foreach (CobraMirageSpit spit in m_mirageCobras)
        {
            if (spit.gameObject == this.gameObject)
            {
                continue;
            }

            spit.EnterPot();
        }
    }

    private void LowerHeads()
    {
        CobraHealth.SetCollider(true);
        MessageBus.TriggerEvent(EMessageType.vulnerableStart);

        m_spit.LowerHead();

        foreach (CobraMirageSpit mirageCobra in m_mirageCobras)
        {
            mirageCobra.LowerHead();
        }
    }

    private void RaiseHeads()
    {
        CobraHealth.SetCollider(false);
        MessageBus.TriggerEvent(EMessageType.vulnerableEnd);

        m_spit.RaiseHead();

        foreach (CobraMirageSpit mirageCobra in m_mirageCobras)
        {
            if (mirageCobra != null)
            {
                mirageCobra.RaiseHead();
            }
        }
    }

    private void GenerateSnakes()
    {
        // Store a list of the pots that should fire
        List<int> firingPots = CobraHealth.StateSettings.m_barrageAttackPositions;

        m_mirageCobras.Clear();

        foreach (int cobra in firingPots)
        {
            m_mirageCobras.Add(s_boss.m_cobraPots[cobra].GetComponent<CobraMirageSpit>());
        }
    }

    private IEnumerator FireProjectiles()
    {
        // Enable collider, can be damaged
        LowerHeads();

        while (s_shotsFired < CobraHealth.StateSettings.m_barrageProjectilesPerHead)
        {
            yield return null;
        }

        //// Fire all heads, at a delay
        //for (int i = 0; i < CobraHealth.StateSettings.m_barrageProjectilesPerHead; i++)
        //{
        //    FireAllHeads();

        //    yield return new WaitForSeconds(CobraHealth.StateSettings.m_barrageProjectileInterval);
        //}

        yield return new WaitForSeconds(0.5f);

        OnAttackEnd();
        CompleteBehaviour();
    }

    public void CancelAttack()
    {
        StopAllCoroutines();
        OnAttackEnd();
        CompleteBehaviour();
    }

    private void FireAllHeads()
    {
        foreach(CobraMirageSpit spit in m_mirageCobras)
        {
            if (spit != null)
            {
                spit.FireProjectile();
            }
        }

        m_spit.FireProjectile();
    }

    private void OnAttackEnd()
    {
        // Disable collider
        RaiseHeads();

        EnterPots();
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