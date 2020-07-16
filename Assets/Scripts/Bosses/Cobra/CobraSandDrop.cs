using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraSandDrop : CobraBehaviour
{
    public Transform m_arenaCenter;
    public GameObject m_sandPrefab;
    public List<CobraPot> m_pots;

    private Vector3 m_arenaTopLeft;
    private List<int> m_potFiringOrder;

    private void Awake()
    {
        // Find position of the top left of the arena
        m_arenaTopLeft = m_arenaCenter.position;
        m_arenaTopLeft += Vector3.forward * 2.0f;
        m_arenaTopLeft += -Vector3.right * 2.0f;

    }

    private void GeneratePotFiringOrder()
    {
        m_potFiringOrder = new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int i = 0; i < m_potFiringOrder.Count; i++)
        {
            int temp = m_potFiringOrder[i];
            int randomIndex = Random.Range(i, m_potFiringOrder.Count);
            m_potFiringOrder[i] = m_potFiringOrder[randomIndex];
            m_potFiringOrder[randomIndex] = temp;
        }
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();

        GeneratePotFiringOrder();

        StartCoroutine(StartScramble());
    }

    private IEnumerator StartScramble()
    {
        yield return new WaitForSeconds(CobraBoss.m_settings.m_timeBeforeGenerate);

        // Choose a random layout from the list and generate it
        int layoutIndex = Random.Range(0, CobraBoss.m_settings.m_blockLayouts.Count);
        GenerateBlockScramble(CobraBoss.m_settings.m_blockLayouts[layoutIndex]);

        for (int i = 0; i < CobraHealth.StateSettings.m_numPotsToFire; i++)
        {
            // One pot fires its group of projectiles
            for (int j = 0; j < CobraHealth.StateSettings.m_projectilesPerPot; j++)
            {
                m_pots[m_potFiringOrder[i]].FireProjectile();
                yield return new WaitForSeconds(CobraHealth.StateSettings.m_potProjectileInterval);
            }

            yield return new WaitForSeconds(CobraHealth.StateSettings.m_delayBetweenPots);
        }

        CompleteBehaviour();
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();
    }

    public override void Reset()
    {
        base.Reset();
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
                    case 'S':
                    case 's':
                        {
                            generatedBlock = Instantiate(m_sandPrefab, worldPosition, Quaternion.identity, transform.parent);
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
                    CobraStateSettings settings = CobraHealth.StateSettings;
                    float lifetime = settings.m_numPotsToFire * settings.m_delayBetweenPots + settings.m_numPotsToFire * settings.m_projectilesPerPot * settings.m_potProjectileInterval;
                    Destroy(generatedBlock, lifetime);
                }
            }
        }
    }
}
