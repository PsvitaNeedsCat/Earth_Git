using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Dialogue : Interactable
{
    // Variables
    [SerializeField] private string[] m_dialogue;
    private int m_dialogueIndex = 0;
    private char[] m_curDialogue;
    private int m_charIndex = 0;
    private string m_displayText = "";
    private bool m_active = false;

    // References
    private GameObject m_dialoguePrefab;
    private GameObject m_dialogueObj = null;
    private TextMeshProUGUI m_dialogueText = null;
    private PlayerInput m_player;

    // Timer
    private float m_timer = 0.0f;
    private const float m_timerMax = 0.04f;

    public override void Awake()
    {
        base.Awake();
        m_dialoguePrefab = Resources.Load<GameObject>("Prefabs/DialogueCanvas");
        m_player = FindObjectOfType<PlayerInput>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        MessageBus.AddListener(EMessageType.continueDialogue, ContinueDialogue);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        MessageBus.RemoveListener(EMessageType.continueDialogue, ContinueDialogue);
    }

    public override void Invoke()
    {
        m_player.SetDialogue(true);

        // Instantiate dialogue
        m_dialogueObj = Instantiate(m_dialoguePrefab, Vector3.zero, Quaternion.identity);
        m_dialogueText = m_dialogueObj.GetComponentInChildren<TextMeshProUGUI>();

        m_curDialogue = m_dialogue[m_dialogueIndex].ToCharArray();

        m_active = true;
    }

    private void Update()
    {
        if (!m_active) { return; }

        // Go through current dialogue
        if (m_timer <= 0.0f)
        {
            m_timer += m_timerMax;

            // Display next char
            if (m_charIndex < m_curDialogue.Length)
            {
                m_displayText += m_curDialogue[m_charIndex];

                // Update text object
                m_dialogueText.text = m_displayText;

                m_charIndex += 1;
            }
        }
        else { m_timer -= Time.deltaTime; }
    }

    public void ContinueDialogue(string _null)
    {
        if (!m_active) { return; }

        // If text is still being 'typed'
        if (m_charIndex < m_curDialogue.Length)
        {
            m_charIndex = m_curDialogue.Length;
            m_displayText = m_dialogue[m_dialogueIndex];
            m_dialogueText.text = m_displayText;
        }
        else
        {
            // Move to next dialogue
            NextDialogue();
        }
    }

    // Moves onto the next chunks of dialogue, or closes the dialogue
    private void NextDialogue()
    {
        // If last dialogue is currently being displayed
        if (m_dialogueIndex == m_dialogue.Length - 1)
        {
            CloseDialogue();
        }
        else
        {
            // Display next piece of dialogue
            m_dialogueIndex += 1;
            m_charIndex = 0;
            m_displayText = "";
            m_curDialogue = m_dialogue[m_dialogueIndex].ToCharArray();
        }
    }

    private void CloseDialogue()
    {
        m_active = false;
        m_timer = 0.0f;
        Destroy(m_dialogueObj);
        m_dialogueObj = null;
        m_dialogueText = null;
        m_displayText = "";
        m_player.SetDialogue(false);
        m_charIndex = 0;
        m_dialogueIndex = 0;
    }
}
