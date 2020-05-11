using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    private System.Action OnHurt;
    private System.Action OnHealed;
    private System.Action OnDeath;

    public enum EHealthType { player, enemy, boss }
    public EHealthType m_type;

    private int m_maxHealth = int.MaxValue;

    private bool m_isDead = false;
    public bool IsInvincible { get; set; } = false;
    private bool m_timerActive = false;
    private float m_invincibleTimer = 0.0f;

    private int m_curHealth = 1;
    public int Health
    {
        get { return m_curHealth; }
        set
        {
            // Disable set behaviour once dead
            if (m_isDead) return;

            // Check if health changed, and call appropriate callbacks
            int delta = value - m_curHealth;

            // If invincible, cannot be damaged, but can still be healed
            if (IsInvincible && delta < 0) { delta = 0; }
            if (delta > 0) OnHealed?.Invoke();
            if (delta < 0) OnHurt?.Invoke();

            // Update health, and check for death
            m_curHealth += delta;
            m_curHealth = Mathf.Clamp(m_curHealth, 0, m_maxHealth);
            if (m_curHealth == 0)
            {
                m_isDead = true;
                OnDeath?.Invoke();
            }
        }
    }

    private void Update()
    {
        // Invincibiity timer
        if (m_timerActive)
        {
            if (m_invincibleTimer <= 0.0f)
            {
                m_timerActive = false;
                IsInvincible = false;
            }
            else { m_invincibleTimer -= Time.deltaTime; }
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

    // Sets the component as invincible for a set time
    public void SetInvincibleTimer(float _time)
    {
        m_invincibleTimer = _time;
        IsInvincible = true;
        m_timerActive = true;
    }
}
