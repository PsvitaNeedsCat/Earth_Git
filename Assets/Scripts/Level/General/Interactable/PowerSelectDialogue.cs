using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class PowerSelectDialogue : Dialogue
{
    [SerializeField] private GameObject[] m_gemSprites = new GameObject[] { };
    private Vector2[] m_gemPositions = new Vector2[]
    {
        new Vector2(536.0f, -319.0f),
        new Vector2(725.0f, -131.0f),
        new Vector2(351.0f, -129.0f),
        new Vector2(539.0f, 56.0f),
    };
    [SerializeField] private UnityEvent m_completedEvent = new UnityEvent();

    private EChunkEffect m_crystalType = EChunkEffect.none;

    public override void Awake()
    {
        m_crystalType = GetComponentInChildren<Crystal>().m_crystalType;

        base.Awake();
    }

    public override void Invoke()
    {
        PlayerInput.s_controls.PlayerCombat.PowerSelection.performed += ctx => PowerEquipped(ctx.ReadValue<Vector2>());

        base.Invoke();

        // Remove A button
        DialogueButtons.EButton button = DialogueButtons.EButton.Rock;
        if (m_crystalType == EChunkEffect.water)
        {
            button = DialogueButtons.EButton.Water;
        }
        else if (m_crystalType == EChunkEffect.fire)
        {
            button = DialogueButtons.EButton.Fire;
        }
        m_dialogueObj.GetComponentInChildren<DialogueButtons>().ChangeActiveButton(button);

        m_player.SetCombat(true);
    }

    public override void ContinueDialogue(string _null)
    {
        if (!m_active)
        {
            return;
        }

        // If text is still being 'typed'
        if (m_charIndex < m_curDialogue.Length)
        {
            m_charIndex = m_curDialogue.Length;
            m_displayText = m_dialogue[m_dialogueIndex];
            m_dialogueText.text = m_displayText;
        }
    }

    private void PowerEquipped(Vector2 _dpadDir)
    {
        EChunkEffect effect = EChunkEffect.none;
        if (_dpadDir == Vector2.left) // Water
        {
            effect = EChunkEffect.water;
        }
        else if (_dpadDir == Vector2.right) // Fire
        {
            effect = EChunkEffect.fire;
        }
        else if (_dpadDir == Vector2.down) // Mirage
        {
            return;
        }
        if (effect != m_crystalType)
        {
            return;
        }

        GameObject gem = m_gemSprites[(int)effect];

        // Enable sprite
        gem.GetComponent<Image>().enabled = true;
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        // Set size and scale
        RectTransform gemTransform = gem.GetComponent<RectTransform>();
        gemTransform.localScale = Vector3.one;
        gemTransform.anchoredPosition = m_gemPositions[(int)effect];

        // Animate sprite
        Sequence animation = DOTween.Sequence();
        animation.Append(gemTransform.DOLocalMove(Vector3.zero, 1.0f));
        animation.Insert(0.0f, gemTransform.DOScale(Vector3.one * 0.2f, 0.5f));
        animation.OnComplete(() => AnimationComplete(effect));

        PlayerInput.s_controls.PlayerCombat.PowerSelection.performed -= ctx => PowerEquipped(ctx.ReadValue<Vector2>());
    }

    private void AnimationComplete(EChunkEffect _effect)
    {
        Player player = FindObjectOfType<Player>();
        player.PowerUnlocked(_effect, true);
        player.TryChangeEffect(_effect);

        m_completedEvent.Invoke();
    }
}
