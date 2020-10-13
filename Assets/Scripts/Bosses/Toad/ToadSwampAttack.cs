using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadSwampAttack : ToadBehaviour
{
    public ToadWave m_toadWave;

    [SerializeField] private Transform m_splashParticlesSpawn = null;

    private float m_startingX;
    private float[] m_positionSeq;
    private int m_positionIndex = 0;
    private Collider m_collider;
    private ToadBossSettings m_toadSettings;

    private void Awake()
    {
        // Get x position from parent
        m_startingX = transform.parent.position.x;

        // Possible places to move to after wave attack
        m_positionSeq = new float[] { m_startingX + Grid.s_tileSize, m_startingX - Grid.s_tileSize, m_startingX };

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

        float newXPos = m_positionSeq[m_positionIndex];
        ++m_positionIndex;
        if (m_positionIndex >= m_positionSeq.Length)
        {
            m_positionIndex = 0;
        }

        Vector3 oldPos = transform.parent.position;
        oldPos.x = newXPos;
        transform.parent.position = oldPos;

        yield return new WaitForSeconds(m_toadSettings.m_underwaterTime / 2.0f);

        m_toadAnimator.SetTrigger("SwampAttackFinish");
    }

    public void LaunchWave()
    {
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
