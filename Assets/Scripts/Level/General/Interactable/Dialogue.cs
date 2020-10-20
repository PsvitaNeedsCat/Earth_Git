using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

[System.Serializable]
public class Dialogue : Interactable
{
    // Variables
    [SerializeField] private Sprite m_characterSprite;
    [SerializeField] private string m_name = "";
    [SerializeField] private bool m_playOnAwake = false;
    [SerializeField] private bool m_introDialogue = false;
    [TextArea(5, 5)] public string[] m_dialogue;
    [SerializeField] private UnityEvent m_endEvent = new UnityEvent();

    protected int m_dialogueIndex = 0;
    protected char[] m_curDialogue;
    protected int m_charIndex = 0;
    protected string m_displayText = "";
    protected bool m_active = false;
    private const float m_marginWithSprite = 462.927f;

    // References
    private GameObject m_dialoguePrefab;
    protected GameObject m_dialogueObj = null;
    protected TextMeshProUGUI m_dialogueText = null;
    protected PlayerInput m_player;

    // Timer
    private float m_timer = 0.0f;
    private const float m_timerMax = 0.03f;

    public override void Awake()
    {
        base.Awake();
        m_dialoguePrefab = Resources.Load<GameObject>("Prefabs/DialogueCanvas");
        m_player = FindObjectOfType<PlayerInput>();
    }

    private void Start()
    {
        if (m_playOnAwake)
        {
            Invoke();
        }
    }

    public override void OnEnable()
    {
        if (m_prompt)
        {
            base.OnEnable();
        }
        MessageBus.AddListener(EMessageType.continueDialogue, ContinueDialogue);
    }
    public override void OnDisable()
    {
        if (m_prompt)
        {
            base.OnDisable();
        }
        MessageBus.RemoveListener(EMessageType.continueDialogue, ContinueDialogue);
    }

    public override void Invoke()
    {
        if (m_player)
        {
            m_player.SetDialogue(true);
        }

        // Instantiate dialogue
        m_dialogueObj = Instantiate(m_dialoguePrefab, Vector3.zero, Quaternion.identity);

        // Second text is always the correct one
        TextMeshProUGUI[] textMeshes = m_dialogueObj.GetComponentsInChildren<TextMeshProUGUI>();
        m_dialogueText = textMeshes[1];

        // If this is the intro dialogue, remove the  background and move the text box to the center of the screen
        if (m_introDialogue)
        {
            m_dialogueObj.transform.Find("Background").gameObject.SetActive(false);
            m_dialogueObj.transform.Find("Controls").gameObject.SetActive(false);

            m_dialogueText.rectTransform.position += Vector3.up * 230.0f;
            m_dialogueText.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        if (m_characterSprite != null)
        {
            // Character sprite assign
            Image[] images = m_dialogueObj.GetComponentsInChildren<Image>();
            foreach (Image i in images)
            {
                if (i.gameObject.name == "CharacterSprite")
                {
                    i.sprite = m_characterSprite;
                    i.color = Color.white;
                    break;
                }
            }

            // Move text margin
            Vector4 margin = m_dialogueText.margin;
            margin.x = m_marginWithSprite;
            m_dialogueText.margin = margin;

            TextMeshProUGUI secondTMPro = m_dialogueText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            margin = secondTMPro.margin;
            margin.x = m_marginWithSprite;
            secondTMPro.margin = margin;
        }

        // Assign nameplate
        if (m_name != "")
        {
            // Enable image & set text
            for (int i = 0; i < m_dialogueObj.transform.childCount; i++)
            {
                GameObject nameplate = m_dialogueObj.transform.GetChild(i).gameObject;
                if (nameplate.name == "Nameplate")
                {
                    nameplate.GetComponent<Image>().enabled = true;
                    nameplate.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = m_name;
                    break;
                }
            }
        }

        m_curDialogue = (m_dialogue[m_dialogueIndex].Length > 0) ? m_dialogue[m_dialogueIndex].ToCharArray() : "".ToCharArray();

        StartCoroutine(ActivateDialogue());
    }

    public override void Update()
    {
        if (!m_active)
        {
            base.Update();
            return; 
        }

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
        else 
        {
            m_timer -= Time.deltaTime; 
        }
    }

    public virtual void ContinueDialogue(string _null)
    {
        if (!m_active)
        {
            return; 
        }

        // If text is still being 'typed'
        if (m_charIndex < m_curDialogue.Length && m_curDialogue.Length != 0)
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
        if (m_player)
        {
            m_player.SetDialogue(false);
        }
        m_charIndex = 0;
        m_dialogueIndex = 0;
        m_endEvent.Invoke();
    }

    // Returns true if the dialogue is still active
    public bool IsRunning()
    {
        return m_active;
    }

    // Counts for a few milliseconds, then activates the dialogue variable
    private IEnumerator ActivateDialogue()
    {
        yield return null;

        m_active = true;

        if (m_prompt)
        {
            m_prompt.SetActive(false);
        }
    }
}
