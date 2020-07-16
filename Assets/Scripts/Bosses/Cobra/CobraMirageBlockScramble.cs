using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageBlockScramble : CobraBehaviour
{
    public Transform m_arenaCenter;
    public GameObject m_blueBlockPrefab;
    public GameObject m_redBlockPrefab;
    public GameObject m_desertEnemyPrefab;

    private Vector3 m_arenaTopLeft;
    private List<Vector3> m_enemySpawnpoints = new List<Vector3>();
    private List<int> m_enemySpawnOrder;

    private void Awake()
    {
        // Find position of the top left of the arena
        m_arenaTopLeft = m_arenaCenter.position;
        m_arenaTopLeft += Vector3.forward * 2.0f;
        m_arenaTopLeft += -Vector3.right * 2.0f;

        GenerateEnemySpawnpoints();
    }

    private void GenerateEnemySpawnpoints()
    {
        Vector3 farTopLeft = m_arenaTopLeft + Vector3.forward + -Vector3.right;

        for (int i = 0; i < 5; i++)
        {
            m_enemySpawnpoints.Add(farTopLeft + Vector3.right * (i + 1));
            m_enemySpawnpoints.Add(farTopLeft + -Vector3.forward * (i + 1));
        }
    }

    private void GenerateEnemySpawnOrder()
    {
        // Get list of 0-9
        m_enemySpawnOrder = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        
        // Shuffle it
        for (int i = 0; i < m_enemySpawnOrder.Count; i++)
        {
            int temp = m_enemySpawnOrder[i];
            int randomIndex = Random.Range(i, m_enemySpawnOrder.Count);
            m_enemySpawnOrder[i] = m_enemySpawnOrder[randomIndex];
            m_enemySpawnOrder[randomIndex] = temp;
        }
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        // StartCoroutine(CompleteAfterSeconds(3.0f));
        StartCoroutine(StartScramble());
    }

    private IEnumerator StartScramble()
    {
        yield return new WaitForSeconds(CobraBoss.m_settings.m_timeBeforeGenerate);

        // Choose a random layout from the list and generate it
        int layoutIndex = Random.Range(0, CobraBoss.m_settings.m_blockLayouts.Count);
        GenerateBlockScramble(CobraBoss.m_settings.m_blockLayouts[layoutIndex]);

        for (int i = 0; i < CobraHealth.StateSettings.m_enemiesToSpawn; i++)
        {
            SpawnEnemy(i);
            yield return new WaitForSeconds(CobraHealth.StateSettings.m_scrambleEnemySpawnInterval);
        }

        CompleteBehaviour();
    }

    private void SpawnEnemy(int _index)
    {
        GameObject newEnemy = Instantiate(m_desertEnemyPrefab, m_enemySpawnpoints[_index], Quaternion.identity, transform.parent);
        Destroy(newEnemy, CobraHealth.StateSettings.m_enemyLifetime);
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
    }

    public override void Reset()
    {
        base.Reset();

        GenerateEnemySpawnOrder();
    }

    private void GenerateBlockScramble(string _layout)
    {
        int arenaLength = CobraBoss.m_settings.m_arenaLength;

        // If the length of the layout array isn't correct, don't try to load it
        if (_layout.Length != arenaLength * arenaLength)
        {
            Debug.LogError("Cobra mirage block scramble layout didn't have 25 characters");
            return;
        }

        for (int i = 0; i < arenaLength; i++)
        {
            for (int j = 0; j < arenaLength; j++)
            {
                char blockType = _layout[i * arenaLength + j];
                
                // Find where to spawn the block
                Vector3 worldPosition = m_arenaTopLeft + i * Vector3.right + j * -Vector3.forward;
                GameObject generatedBlock = null;

                // Based on the character read, spawn a block, or ignore
                switch (blockType)
                {
                    case 'B':
                    case 'b':
                        {
                            generatedBlock = Instantiate(m_blueBlockPrefab, worldPosition, Quaternion.identity, transform.parent);
                            break;
                        }

                    case 'R':
                    case 'r':
                        {
                            generatedBlock = Instantiate(m_redBlockPrefab, worldPosition, Quaternion.identity, transform.parent);
                            break;
                        }

                    default:
                        {
                            // Don't spawn a block
                            break;
                        }
                }

                // If we generated a block, set it to destroy after a time
                if (generatedBlock != null)
                {
                    Destroy(generatedBlock, CobraHealth.StateSettings.m_scrambleEnemySpawnInterval * CobraHealth.StateSettings.m_enemiesToSpawn);
                }
            }
        }
    }
}
