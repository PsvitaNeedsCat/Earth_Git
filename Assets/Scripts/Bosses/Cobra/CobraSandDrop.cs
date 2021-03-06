﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CobraSandDrop : CobraBehaviour
{
    public Transform m_arenaCenter;
    public GameObject m_sandPrefab;

    private Vector3 m_arenaTopLeft;
    private List<int> m_potFiringOrder;
    private PlayerController m_playerController;
    private CobraBoss m_boss;

    protected override void Awake()
    {
        base.Awake();

        // Find position of the top left of the arena
        m_arenaTopLeft = m_arenaCenter.position;
        m_arenaTopLeft += Vector3.forward * 2.0f;
        m_arenaTopLeft += -Vector3.right * 2.0f;

        m_playerController = FindObjectOfType<PlayerController>();
        m_boss = GetComponent<CobraBoss>();
    }

    private void GeneratePotFiringOrder()
    {
        // Generate a shuffled order of integers
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
        //m_boss.StartFlipTiles();

        //yield return new WaitForSeconds(3.0f);

        m_animations.EnterPot();

        yield return new WaitForSeconds(CobraBoss.s_settings.m_timeBeforeGenerate);

        // Choose a random layout from the list and generate it
        int layoutIndex = Random.Range(0, CobraBoss.s_settings.m_blockLayouts.Count);
        GenerateBlockScramble(CobraBoss.s_settings.m_blockLayouts[layoutIndex]);

        yield return new WaitForSeconds(CobraHealth.StateSettings.m_waitAfterSandDrop);

        // num pot groups to fire

        // num pots at once

        for (int i = 0; i < CobraHealth.StateSettings.m_sandDropNumPotGroups; i++)
        {
            // Store the indices of the pots that will fire in this group, and make them shake to warn the player
            List<int> firingPotIndices = new List<int>();
            for (int x = 0; x < CobraHealth.StateSettings.m_sandDropPotsPerGroup; x++)
            {
                int potIndex = (i * CobraHealth.StateSettings.m_sandDropPotsPerGroup + x) % m_potFiringOrder.Count;
                firingPotIndices.Add(potIndex);
                CobraPot pot = s_boss.m_cobraPots[m_potFiringOrder[potIndex]];
                Debug.Log("Flashing eye " + m_potFiringOrder[potIndex]);
                pot.FlashEye();
                // pot.m_moveTransform.DOPunchScale()
            }

            yield return new WaitForSeconds(1.5f);

            for (int j = 0; j < CobraHealth.StateSettings.m_projectilesPerPot; j++)
            {
                foreach(int potIndex in firingPotIndices)
                {
                    s_boss.m_cobraPots[m_potFiringOrder[potIndex]].FireProjectile();
                }

                //for (int k = 0; k < CobraHealth.StateSettings.m_sandDropPotsPerGroup; k++)
                //{
                //    int potIndex = (i * CobraHealth.StateSettings.m_sandDropPotsPerGroup + k) % m_potFiringOrder.Count;
                //    s_boss.m_cobraPots[m_potFiringOrder[potIndex]].FireProjectile();
                //}

                yield return new WaitForSeconds(CobraHealth.StateSettings.m_potProjectileInterval);
            }

            yield return new WaitForSeconds(CobraHealth.StateSettings.m_delayBetweenPotGroups);
        }

        yield return new WaitForSeconds(1.0f);

        m_animations.Roar();
        
        // m_animations.EnterPot();

        yield return new WaitForSeconds(1.5f);

        CompleteBehaviour();
    }

    public override void CompleteBehaviour()
    {
        base.CompleteBehaviour();

        StopAllCoroutines();

        m_playerController.m_inSand = false;
    }

    public override void Reset()
    {
        base.Reset();
    }

    private void GenerateBlockScramble(string _layout)
    {
        int arenaLength = CobraBoss.s_settings.m_arenaLength;

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
                Vector3 worldPosition = m_arenaTopLeft + i * Vector3.right + j * -Vector3.forward + Vector3.up * CobraBoss.s_settings.m_sandDropHeight;
                GameObject generatedBlock = null;

                // Based on the character read, spawn a block, or ignore
                switch (blockType)
                {
                    case 'S':
                    case 's':
                        {
                            generatedBlock = Instantiate(m_sandPrefab, worldPosition, Quaternion.identity, m_arenaCenter);
                            generatedBlock.GetComponent<SandBlock>().Fall();
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
                    float lifetime = settings.m_sandDropNumPotGroups * settings.m_delayBetweenPotGroups + settings.m_sandDropNumPotGroups * settings.m_projectilesPerPot * settings.m_potProjectileInterval + settings.m_waitAfterSandDrop + settings.m_sandDropNumPotGroups * 1.5f;
                    Destroy(generatedBlock, lifetime);
                }
            }
        }
    }
}
