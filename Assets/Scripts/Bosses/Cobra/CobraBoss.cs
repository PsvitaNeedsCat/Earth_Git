using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the behaviour sequencing of the cobra boss
public class CobraBoss : MonoBehaviour
{
    public static CobraGlobalSettings s_settings;

    public Transform m_arenaCenter;
    public List<CobraBehaviour> m_behaviourLoop;
    public CobraBehaviour m_chaseBehaviour;
    public float m_startDelay = 3.0f;
    public List<CobraPot> m_cobraPots;

    private int m_currentBehaviourIndex = 0;
    private int m_totalBehaviours;
    private int m_behaviourLoopCount = 0;
    private CobraBehaviour m_currentBehaviour;
    private CobraHealth m_cobraHealth;
    private PlayerController m_playerController;
    private List<FlippableTile> m_flippableTiles;
    private static Vector3 m_arenaTopLeft;
    private CobraAnimations m_animations;

    // Initialise variables
    private void Awake()
    {
        // Initialise variables
        m_cobraHealth = GetComponent<CobraHealth>();
        s_settings = Resources.Load<CobraGlobalSettings>("ScriptableObjects/CobraGlobalSettings");
        m_cobraHealth.SetCurrentHealth(s_settings.m_maxHealth);

        m_totalBehaviours = m_behaviourLoop.Count;
        m_playerController = FindObjectOfType<PlayerController>();
        m_animations = GetComponent<CobraAnimations>();

        m_arenaTopLeft = m_arenaCenter.position;
        m_arenaTopLeft += Vector3.forward * 2.0f;
        m_arenaTopLeft += -Vector3.right * 2.0f;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Switch to the chase behaviour, and start chasing the player
    public void StartChase()
    {
        m_currentBehaviour = m_chaseBehaviour;
        m_currentBehaviour.StartBehaviour();
    }

    // Start the first behaviour after a delay
    private void Start()
    {
        m_flippableTiles = new List<FlippableTile>(FindObjectsOfType<FlippableTile>());

        m_currentBehaviour = m_behaviourLoop[0];
        StartCoroutine(DelayedStart());
    }

    // Waits for a delay, and then starts the first behaviour
    private IEnumerator DelayedStart()
    {
        StartFlipTiles();

        yield return new WaitForSeconds(m_startDelay);
        m_currentBehaviour.StartBehaviour();
    }

    private void Update()
    {
        UpdateBehaviour();
    }

    private void UpdateBehaviour()
    {
        // Move to next behaviour when complete
        if (m_currentBehaviour.m_currentState == CobraBehaviour.EBehaviourState.complete)
        {
            GoToNextBehaviour();
        }
    }

    private void GoToNextBehaviour()
    {
        // Reset behaviour we just finished
        m_currentBehaviour.Reset();

        // Move to next behaviour and start it
        m_currentBehaviourIndex = (m_currentBehaviourIndex + 1) % m_totalBehaviours;

        // Check if behaviours have looped
        if (m_currentBehaviourIndex == 0)
        {
            m_behaviourLoopCount++;
        }

        // If behaviour loop count is even, skip the sand drop attack, otherwise skip the mirage wall attack
        if ((m_behaviourLoopCount % 2) == 0 && m_currentBehaviourIndex == 1)
        {
            m_currentBehaviourIndex++;
        }
        else if ((m_behaviourLoopCount % 2) == 1 && m_currentBehaviourIndex == 0)
        {
            m_currentBehaviourIndex++;
        }

        m_currentBehaviour = m_behaviourLoop[m_currentBehaviourIndex];
        m_currentBehaviour.StartBehaviour();
    }

    public void StartFlipTiles()
    {
        m_animations.CobraJump();
    }

    public void FlipTiles()
    {
        StartCoroutine(OnJumpImpact());
    }

    // Knocks up the player, and shortly after, flips over all the tiles
    private IEnumerator OnJumpImpact()
    {
        m_playerController.KnockBack(Vector3.up * 2.5f);
        MessageBus.TriggerEvent(EMessageType.cobraPotBigThud);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);

        

        yield return new WaitForSeconds(0.2f);

        if (m_flippableTiles == null)
        {
            Debug.LogError("Flippable tiles null");
        }
        else
        {
            // Debug.Log("I have " + m_flippableTiles.Count + " flippable tiles");
        }

        for (int i = 0; i < m_flippableTiles.Count; i++)
        {
            if (m_flippableTiles[i] != null)
            {
                m_flippableTiles[i].Flip();
            }
        }

        yield return new WaitForSeconds(1.0f);
        ChunkManager.DestroyAllChunks();
    }

    public void SortPotList()
    {
        m_cobraPots.Sort((potOne, potTwo) => potOne.m_potIndex.CompareTo(potTwo.m_potIndex));

        for (int i = 0; i < m_cobraPots.Count; i++)
        {
            m_cobraPots[i].m_potIndex = i;
        }
    }


    // Tile layout
    // 0 1 2 3 4
    // 5 6 7 8 9    etc
    public static Vector3 GetTileWorldPos(int _index)
    {
        int dX = _index % 5;
        int dY = _index / 5;

        return m_arenaTopLeft + Vector3.right * dX + -Vector3.forward * dY;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
