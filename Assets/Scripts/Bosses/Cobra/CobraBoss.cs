using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraBoss : MonoBehaviour
{
    public static CobraGlobalSettings m_settings;

    public List<CobraBehaviour> m_behaviourLoop;
    public CobraBehaviour m_chaseBehaviour;
    public float m_startDelay = 3.0f;

    private int m_currentBehaviourIndex = 0;
    private int m_totalBehaviours;
    private CobraBehaviour m_currentBehaviour;
    private CobraHealth m_cobraHealth;
    private bool m_chasing = false;
    private PlayerController m_playerController;
    private List<FlippableTile> m_flippableTiles;

    // Initialise variables
    private void Awake()
    {
        m_cobraHealth = GetComponent<CobraHealth>();

        m_settings = Resources.Load<CobraGlobalSettings>("ScriptableObjects/CobraGlobalSettings");
        m_cobraHealth.SetCurrentHealth(m_settings.m_maxHealth);

        m_totalBehaviours = m_behaviourLoop.Count;
        m_playerController = FindObjectOfType<PlayerController>();
        m_flippableTiles = new List<FlippableTile>(FindObjectsOfType<FlippableTile>());
    }

    public void StartChase()
    {
        m_chasing = true;
        m_currentBehaviour = m_chaseBehaviour;
        m_currentBehaviour.StartBehaviour();
    }

    // Start first behaviour
    private void Start()
    {
        m_currentBehaviour = m_behaviourLoop[0];
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
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
        m_currentBehaviour = m_behaviourLoop[m_currentBehaviourIndex];
        m_currentBehaviour.StartBehaviour();
    }

    public void FlipTiles()
    {
        StartCoroutine(StartTileFlip());        
    }

    private IEnumerator StartTileFlip()
    {
        m_playerController.KnockBack(Vector3.up * 2.5f);

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < m_flippableTiles.Count; i++)
        {
            m_flippableTiles[i].Flip();
        }
    }
}
