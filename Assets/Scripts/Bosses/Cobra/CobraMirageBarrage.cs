using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageBarrage : CobraBehaviour
{
    public List<CobraPot> m_pots;

    private GameObject m_mirageClonePrefab;
    private List<CobraMirageSpit> m_mirageCobras = new List<CobraMirageSpit>();
    private CobraMirageSpit m_spit;
    private CobraBoss m_boss;

    private void Awake()
    {
        m_mirageClonePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/MirageCobra");
        m_spit = GetComponent<CobraMirageSpit>();
        m_boss = GetComponent<CobraBoss>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        StartCoroutine(StartSpawning());        
    }

    private IEnumerator StartSpawning()
    {
        m_boss.FlipTiles();

        yield return new WaitForSeconds(1.0f);

        // Create mirage clones, and place them around the pots
        GenerateSnakes();

        yield return new WaitForSeconds(2.0f);

        RaiseHead();

        StartCoroutine(FireProjectiles());
    }

    private void LowerHead()
    {
        CobraHealth.SetCollider(true);
    }

    private void RaiseHead()
    {
        CobraHealth.SetCollider(false);
    }

    private void GenerateSnakes()
    {
        // Choose order
        List<int> spawnPots = CobraHealth.StateSettings.m_barrageAttackPositions;

        // The pot in which to have the real snake
        int realSpawnPot = spawnPots[Random.Range(0, spawnPots.Count)];
        transform.parent.position = m_pots[realSpawnPot].transform.position;
        transform.parent.rotation = m_pots[realSpawnPot].transform.rotation;
        GetComponent<CobraMirageSpit>().m_bulletType = (realSpawnPot % 2 == 0) ? ECobraMirageType.blue : ECobraMirageType.red;

        for (int i = 0; i < spawnPots.Count; i++)
        {
            // The real snake has been spawned here, skip it
            if (spawnPots[i] == realSpawnPot)
            {
                continue;
            }

            Vector3 spawnPos = m_pots[spawnPots[i]].transform.position;
            Quaternion spawnRot = m_pots[spawnPots[i]].transform.rotation;
            GameObject mirageCobra = Instantiate(m_mirageClonePrefab, spawnPos, spawnRot, transform.parent.parent);
            m_mirageCobras.Add(mirageCobra.GetComponent<CobraMirageSpit>());
            mirageCobra.GetComponent<CobraMirageSpit>().m_bulletType = (i % 2 == 0) ? ECobraMirageType.blue : ECobraMirageType.red;
        }
    }

    private IEnumerator FireProjectiles()
    {
        LowerHead();

        for (int i = 0; i < CobraHealth.StateSettings.m_barrageProjectilesPerHead; i++)
        {
            FireAllHeads();

            yield return new WaitForSeconds(CobraHealth.StateSettings.m_barrageProjectileInterval);
        }

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
        RaiseHead();

        for (int i = 0; i < m_mirageCobras.Count; i++)
        {
            if (m_mirageCobras[i] != null)
            {
                Destroy(m_mirageCobras[i].gameObject);
            }
        }

        m_mirageCobras.Clear();
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