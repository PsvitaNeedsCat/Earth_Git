﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadSwampAttack : ToadBehaviour
{
    public ToadWave m_toadWave;

    private float m_startingX;
    private float[] m_possiblePositions;
    private Collider m_collider;
    private ToadBossSettings m_toadSettings;

    private void Awake()
    {
        // Get x position from parent
        m_startingX = transform.parent.position.x;

        // Possible places to move to after wave attack
        m_possiblePositions = new float[] { m_startingX - Grid.m_tileSize, m_startingX, m_startingX + Grid.m_tileSize };
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

        float newXPos = m_possiblePositions[Random.Range(0, 3)];

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
