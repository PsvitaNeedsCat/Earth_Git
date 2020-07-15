using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraMirageBlockScramble : CobraBehaviour
{
    public string m_blockLayout; // B = Blue block, R = Red block, N = None
    public Transform m_arenaCenter;
    public GameObject m_blueBlockPrefab;
    public GameObject m_redBlockPrefab;

    private Vector3 m_arenaTopLeft;

    private void Awake()
    {
        // Find position of the top left of the arena
        m_arenaTopLeft = m_arenaCenter.position;
        m_arenaTopLeft += Vector3.forward * 2.0f;
        m_arenaTopLeft += -Vector3.right * 2.0f;
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

        GenerateBlockScramble(m_blockLayout);

        yield return new WaitForSeconds(CobraBoss.m_settings.m_generatedBlockLifetime + CobraBoss.m_settings.m_timeAfterGenerate);

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
                    Destroy(generatedBlock, CobraBoss.m_settings.m_generatedBlockLifetime);
                }
            }
        }
    }
}
