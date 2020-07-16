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

    private void Awake()
    {
        m_wallDirections = new Vector3[] { transform.forward, transform.right, -transform.forward, -transform.right };
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        StartCoroutine(WallSequence());

        // StartCoroutine(CompleteAfterSeconds(3.0f));
    }

    // Handles the sequencing of all walls
    private IEnumerator WallSequence()
    {
        CobraMirageWallDef[] wallSequence = CobraHealth.StateSettings.m_wallSequence;

        for (int i = 0; i < wallSequence.Length; i++)
        {
            CobraMirageWallDef wallDef = wallSequence[i];

            // Send first wall
            StartCoroutine(SendWall(wallDef.m_wallOneType, wallDef.m_wallOneFrom));

            // Check if there is a second wall, and if so, send it
            if (wallDef.m_wallTwoType != ECobraMirageWallType.none)
            { 
                StartCoroutine(SendWall(wallDef.m_wallTwoType, wallDef.m_wallTwoFrom)); 
            }

            yield return new WaitForSeconds(CobraHealth.StateSettings.m_timeBetweenWalls);
        }

        CompleteBehaviour();

    }

    // Handles the sending of one wall
    private IEnumerator SendWall(ECobraMirageWallType _type, EDirection _direction)
    {
        if (_type == ECobraMirageWallType.none)
        {
            Debug.LogError("Tried to send a cobra mirage wall of type none");
            yield break;
        }

        GameObject wall;

        // Wall appears
        wall = (_type == ECobraMirageWallType.blue) ? m_blueWall : m_redWall;
        wall.transform.position = m_arenaCenter.transform.position + m_wallDirections[(int)_direction] * CobraBoss.m_settings.m_wallSpawnDistance;
        wall.transform.LookAt(m_arenaCenter.transform);
        wall.SetActive(true);

        // Delay before moving
        yield return new WaitForSeconds(CobraHealth.StateSettings.m_wallDelayBeforeMove);

        // Walls moves across the arena
        // wall.transform.DOMoveZ(wall.transform.position.z + CobraBoss.m_settings.m_wallTravelDistance, CobraHealth.StateSettings.m_wallMoveDuration);
        wall.transform.DOBlendableMoveBy(wall.transform.forward * CobraBoss.m_settings.m_wallTravelDistance, CobraHealth.StateSettings.m_wallMoveDuration);
        yield return new WaitForSeconds(CobraHealth.StateSettings.m_wallMoveDuration);

        // Wall disappears
        wall.SetActive(false);
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
