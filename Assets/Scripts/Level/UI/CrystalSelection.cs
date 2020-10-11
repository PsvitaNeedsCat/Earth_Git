using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSelection : MonoBehaviour
{
    [SerializeField] private Sprite[] m_backgroundSprites;
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private Image[] m_gemSprites;

    private int m_currentlySelected = 0;

    private float m_selectedLightValue = 1.0f;
    private float m_defaultLightValue = 0.5f;

    public void UpdateSelected(int _selected, bool _silent = false)
    {
        m_backgroundImage.sprite = m_backgroundSprites[_selected];

        SetGemColour(m_currentlySelected, false);
        SetGemColour(_selected, true);
        m_currentlySelected = _selected;

        if (!_silent)
        {
            m_gemSprites[_selected].rectTransform.DORewind();
            m_gemSprites[_selected].rectTransform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }
    }

    private void SetGemColour(int _gem, bool _selected)
    {
        Color newColour = m_gemSprites[_gem].color;
        float h, s, v;
        Color.RGBToHSV(newColour, out h, out s, out v);
        v = (_selected) ? m_selectedLightValue : m_defaultLightValue;
        newColour = Color.HSVToRGB(h, s, v);
        m_gemSprites[_gem].color = newColour;
    }

    public void UpdateUnlocked(bool[] _active)
    {
        // Enable the images when they are unlocked
        for (int i = 0; i < m_gemSprites.Length; i++)
        {
            m_gemSprites[i].enabled = _active[i];
            SetGemColour(i, (m_currentlySelected == i));
        }
    }
}
