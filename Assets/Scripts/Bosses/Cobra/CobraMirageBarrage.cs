using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageBarrage : CobraBehaviour
{
    public List<CobraPot> m_pots;

    private GameObject m_mirageClonePrefab;
    private List<CobraMirageSpit> m_mirageCobras = new List<CobraMirageSpit>();
    private CobraMirageSpit m_spit;

    private void Awake()
    {
        m_mirageClonePrefab = Resources.Load<GameObject>("Prefabs/Bosses/Cobra/MirageCobra");
        m_spit = GetComponent<CobraMirageSpit>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        StartCoroutine(StartSpawning());        
    }

    private IEnumerator StartSpawning()
    {
        // Create mirage clones, and place them around the pots
        GenerateSnakes();

        yield return new WaitForSeconds(2.0f);

        RaiseHeads();

        StartCoroutine(FireProjectiles());
    }

    private void LowerHeads()
    {
        CobraHealth.SetCollider(true);
    }

    private void RaiseHeads()
    {
        CobraHealth.SetCollider(true);
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
        LowerHeads();

        for (int i = 0; i < CobraHealth.StateSettings.m_barrageProjectilesPerHead; i++)
        {
            FireAllHeads();

            yield return new WaitForSeconds(CobraHealth.StateSettings.m_barrageProjectileInterval);
        }

        yield return new WaitForSeconds(0.5f);

        OnAttackEnd();
        CompleteBehaviour();
    }

    private void FireAllHeads()
    {
        foreach(CobraMirageSpit spit in m_mirageCobras)
        {
            spit.FireProjectile();
        }

        m_spit.FireProjectile();
    }

    private void OnAttackEnd()
    {
        for (int i = 0; i < m_mirageCobras.Count; i++)
        {
            Destroy(m_mirageCobras[i].gameObject);
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