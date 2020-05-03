using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    private System.Action OnHurt;
    private System.Action OnHealed;
    private System.Action OnDeath;

    private int m_maxHealth = int.MaxValue;

    private bool isDead = false;

    private int m_curHealth = 1;
    public int Health
    {
        get { return m_curHealth; }
        set
        {
            // Disable set behaviour once dead
            if (isDead) return;

            // Check if health changed, and call appropriate callbacks
            int delta = Mathf.Clamp(m_curHealth - value, 0, m_maxHealth);
            if (delta > 0) OnHealed?.Invoke();
            if (delta < 0) OnHurt?.Invoke();

            // Update health, and check for death
            m_curHealth += delta;
            if (m_curHealth == 0)
            {
                isDead = true;
                OnDeath?.Invoke();
            }
        }
    }

    // Initialise health component with current and max health
    public void Init(int _current, int _max, System.Action _onHurt, System.Action _onHealed, System.Action _onDeath)
    {
        m_curHealth = Mathf.Clamp(_current, 0, _max);
        m_maxHealth = _max;

        OnHurt = _onHurt;
        OnHealed = _onHealed;
        OnDeath = _onDeath;
    }

    // Initialise health - assume current is max
    public void Init(int _max)
    {
        Debug.Assert(_max >= 0, "Health cannot be negative during initialisation");

        m_curHealth = _max;
        m_maxHealth = _max;
    }
}
