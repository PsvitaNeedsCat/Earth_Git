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
        // Flip tiles and destroy chunks
        m_boss.FlipTiles();

        yield return new WaitForSeconds(0.2f);

        ChunkManager.DestroyAllChunks();

        yield return new WaitForSeconds(1.0f);

        // Store pots to fire
        GenerateSnakes();

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

        //// Choose order
        //List<int> spawnPots = CobraHealth.StateSettings.m_barrageAttackPositions;

        //// The pot in which to have the real snake
        //int realSpawnPot = CobraShuffle.s_bossPotIndex;
        //                                                                                // Band aid
        //transform.parent.position = m_shuffle.m_pots[realSpawnPot].transform.position + Vector3.up * 0.75f;
        //transform.parent.rotation = m_shuffle.m_pots[realSpawnPot].transform.rotation;
        //GetComponent<CobraMirageSpit>().m_bulletType = (realSpawnPot % 2 == 0) ? ECobraMirageType.blue : ECobraMirageType.red;

        //for (int i = 0; i < spawnPots.Count; i++)
        //{
        //    // The real snake has been spawned here, skip it
        //    if (spawnPots[i] == realSpawnPot)
        //    {
        //        continue;
        //    }

        //    // Create a mirage cobra
        //    Vector3 spawnPos = m_shuffle.m_pots[spawnPots[i]].transform.position + Vector3.up * 0.75f;
        //    Quaternion spawnRot = m_shuffle.m_pots[spawnPots[i]].transform.rotation;
        //    GameObject mirageCobra = Instantiate(m_mirageClonePrefab, spawnPos, spawnRot, transform.parent.parent);
        //    m_mirageCobras.Add(mirageCobra.GetComponent<CobraMirageSpit>());
        //    mirageCobra.GetComponent<CobraMirageSpit>().m_bulletType = (i % 2 == 0) ? ECobraMirageType.blue : ECobraMirageType.red;
        //}
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