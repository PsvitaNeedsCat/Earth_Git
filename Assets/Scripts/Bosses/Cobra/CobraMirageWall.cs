using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraMirageWall : CobraBehaviour
{
    public GameObject m_blueWall;
    public GameObject m_redWall;
    public Transform m_arenaCenter;

    private Vector3[] m_wallDirections;
    private CobraBoss m_boss;

    protected override void Awake()
    {
        base.Awake();

        m_wallDirections = new Vector3[] { transform.forward, transform.right, -transform.forward, -transform.right };
        m_boss = GetComponent<CobraBoss>();
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        StartCoroutine(WallSequence());
    }

    // Handles the sequencing of all walls
    private IEnumerator WallSequence()
    {
        // m_boss.StartFlipTiles();

        yield return new WaitForSeconds(0.2f);

        // ChunkManager.DestroyAllChunks();
        yield return new WaitForSeconds(1.0f);

        CobraMirageWallDef[] wallSequence = CobraHealth.StateSettings.m_wallSequence;

        for (int i = 0; i < wallSequence.Length; i++)
        {
            CobraMirageWallDef wallDef = wallSequence[i];

            // Send first wall
            StartCoroutine(SendWall(wallDef.m_wallOneType, wallDef.m_wallOneFrom));

            // Check if there is a second wall, and if so, send it
            if (wallDef.m_wallTwoType != ECobraMirageType.none)
            {
                yield return new WaitForSeconds(CobraHealth.StateSettings.m_wallStaggerTime);
                StartCoroutine(SendWall(wallDef.m_wallTwoType, wallDef.m_wallTwoFrom));
            }

            m_animations.MirageWall();

            yield return new WaitForSeconds(CobraHealth.StateSettings.m_timeBetweenWalls);
        }

        CompleteBehaviour();
    }

    // Handles the sending of one wall
    private IEnumerator SendWall(ECobraMirageType _type, EDirection _direction)
    {
        if (_type == ECobraMirageType.none)
        {
            Debug.LogError("Tried to send a cobra mirage wall of type none");
            yield break;
        }

        MessageBus.TriggerEvent(EMessageType.cobraMirageWall);

        GameObject wall;

        // Wall appears
        wall = (_type == ECobraMirageType.blue) ? m_blueWall : m_redWall;
        wall.transform.position = m_arenaCenter.transform.position + m_wallDirections[(int)_direction] * CobraBoss.s_settings.m_wallSpawnDistance;
        wall.transform.LookAt(m_arenaCenter.transform);
        wall.SetActive(true);

        // Delay before moving
        yield return new WaitForSeconds(CobraHealth.StateSettings.m_wallDelayBeforeMove);

        // Walls moves across the arena
        wall.transform.DOBlendableMoveBy(wall.transform.forward * CobraBoss.s_settings.m_wallTravelDistance, CobraHealth.StateSettings.m_wallMoveDuration);
        yield return new WaitForSeconds(CobraHealth.StateSettings.m_wallMoveDuration);

        // Wall disappears
        wall.SetActive(false);
    }

    public override void CompleteBehaviour()
    {
        // m_animations.EnterPot();

        base.CompleteBehaviour();

        StopAllCoroutines();
    }

    public override void Reset()
    {
        base.Reset();
    }
}
