using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadSwampAttack : ToadBehaviour
{
    public ToadWave m_toadWave;

    [SerializeField] private Transform m_splashParticlesSpawn = null;

    private float m_startingX;
    private float[] m_possiblePositions;
    private List<float> m_positionSeq = new List<float>();
    private Collider m_collider;
    private ToadBossSettings m_toadSettings;

    private void Awake()
    {
        // Get x position from parent
        m_startingX = transform.parent.position.x;

        // Possible places to move to after wave attack
        m_possiblePositions = new float[] { m_startingX - Grid.s_tileSize, m_startingX, m_startingX + Grid.s_tileSize };
        m_collider = GetComponent<Collider>();
        m_toadSettings = Resources.Load<ToadBossSettings>("ScriptableObjects/ToadBossSettings");
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        m_toadAnimator.SetTrigger("SwampAttackStart");
        StartCoroutine(JumpBackOn());
    }

    private IEnumerator JumpBackOn()
    {
        m_collider.isTrigger = true;
        yield return new WaitForSeconds(m_toadSettings.m_underwaterTime / 2.0f);

        // If all positions are used, get new pattern
        if (m_positionSeq.Count == 0)
        {
            GetRandomSequence();
        }

        float newXPos = m_positionSeq[0];
        m_positionSeq.RemoveAt(0);

        Vector3 oldPos = transform.parent.position;
        oldPos.x = newXPos;
        transform.parent.position = oldPos;

        yield return new WaitForSeconds(m_toadSettings.m_underwaterTime / 2.0f);

        m_toadAnimator.SetTrigger("SwampAttackFinish");
    }

    // Gets random positons
    private void GetRandomSequence()
    {
        Dictionary<float, bool> usedPositions = new Dictionary<float, bool>()
        {
            { m_possiblePositions[0], false },
            { m_possiblePositions[1], false },
            { m_possiblePositions[2], false }
        };

        for (int i = 0; i < m_possiblePositions.Length; i++)
        {
            // Get list of possible positions to choose from
            List<float> possiblePositions = new List<float>();
            for (int j = 0; j < m_possiblePositions.Length; j++)
            {
                if (!usedPositions[m_possiblePositions[j]])
                {
                    possiblePositions.Add(m_possiblePositions[j]);
                }
            }

            // Choose position at random
            float chosenPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];
            usedPositions[chosenPosition] = true;
            m_positionSeq.Add(chosenPosition);
        }
    }

    public void LaunchWave()
    {
        EffectsManager.SpawnEffect(EffectsManager.EEffectType.toadSplash, m_splashParticlesSpawn.position, Quaternion.identity, Vector3.one, 2.0f);

        // Move wave back to initial location
        m_toadWave.transform.localPosition = Vector3.zero;

        // Activate and launch wave
        m_toadWave.gameObject.SetActive(true);
        m_toadWave.Launch();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) LaunchWave();
    }

    public void AELaunchWave() => LaunchWave();
}
