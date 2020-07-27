using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // Public variables
    [HideInInspector] public Tile m_confirmedTile = null;
    [HideInInspector] public bool m_inSand = false;
    [HideInInspector] public Rigidbody m_rigidBody;
    [HideInInspector] public static bool s_saveOnAwake = false;

    // Serialized Variables
    [SerializeField] private GameObject m_hurtboxPrefab;
    [SerializeField] private TileTargeter m_tileTargeter;
    [SerializeField] private SkinnedMeshRenderer m_meshRenderer;
    private List<Image> m_healthImages;
    private List<Image> m_healthBackgroundImages;

    // Private variables
    private PlayerController m_instance;
    private GlobalPlayerSettings m_settings;
    private HealthComponent m_health;
    private PlayerInput m_input;
    private GameObject m_pauseMenu = null;
    private GameObject m_pauseMenuPrefab;
    private EventSystem[] m_eventSystems;
    private GameCanvas m_gameCanvas;

    private void Awake()
    {
        // Only one instance
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("A second instance of PlayerController.cs was instantiated");
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }

        m_gameCanvas = FindObjectOfType<GameCanvas>();
        m_healthImages = m_gameCanvas.m_healthImages;
        m_healthBackgroundImages = m_gameCanvas.m_healthBackgroundImages;

        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");

        SetOutlineColour();

        // Set health
        m_health = GetComponent<HealthComponent>();
        // Set max health based on powers unlocked
        int maxHealth = 2;
        foreach (KeyValuePair<EChunkEffect, bool> i in Player.s_activePowers)
        {
            if (i.Value) { ++maxHealth; }
        }
        m_health.Init(maxHealth, maxHealth, OnHurt, OnHealed, OnDeath);
        SetMaxHealth(maxHealth);

        // Set rigidbody
        m_rigidBody = GetComponent<Rigidbody>();
        Debug.Assert(m_rigidBody, "No rigidbody found on player");

        m_input = GetComponent<PlayerInput>();
        Debug.Assert(m_input, "Player has no PlayerInput.cs");

        m_pauseMenuPrefab = Resources.Load<GameObject>("Prefabs/Pause Menu Parent");

        // Save
        if (s_saveOnAwake)
        {
            SaveManager.Instance.SaveGame();
            s_saveOnAwake = false;
        }
    }

    private void Start()
    {
        // Set init spawn location
        if (RoomManager.Instance)
        {
            RoomManager.Instance.m_respawnLocation = transform.position;
        }
    }

    private void OnEnable() => MessageBus.AddListener(EMessageType.fadedToBlackQuiet, AfterDeath);
    private void OnDisable() => MessageBus.RemoveListener(EMessageType.fadedToBlackQuiet, AfterDeath);

    private void OnHurt()
    {
        UpdateHealthSprites();

        MessageBus.TriggerEvent(EMessageType.playerHurt);

        m_health.SetInvincibleTimer(m_settings.m_hurtTime);

        // Tween colour change
        Sequence seq = DOTween.Sequence();
        seq.Append(m_meshRenderer.material.DOColor(m_settings.m_hurtColour, m_settings.m_hurtTime * 0.5f));
        seq.Append(m_meshRenderer.material.DOColor(Color.white, m_settings.m_hurtTime * 0.5f));

        // Tween health bar
        m_healthBackgroundImages[m_health.Health].rectTransform.DORewind();
        m_healthBackgroundImages[m_health.Health].rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.shortSharp);
    }

    private void OnDeath()
    {
        // Freeze player
        m_input.SetCombat(false);
        m_input.SetMovement(false);

        // Fade to black
        RoomManager.Instance.FadeToBlack();
    }

    // Called by message bus after the screen has faded to black
    private void AfterDeath(string _null)
    {
        // Reload the room
        RoomManager.Instance.ReloadCurrentRoom();

        // Reset the player's position
        transform.position = RoomManager.Instance.m_respawnLocation;

        // Reset player's health
        m_health.HealToMax();

        // Fade to game
        RoomManager.Instance.FadeToGame();

        // Unfreeze player
        m_input.SetCombat(true);
        m_input.SetMovement(true);
    }

    private void OnHealed()
    {
        // Update health sprites
        UpdateHealthSprites();

        // Tween health bar
        m_healthBackgroundImages[m_health.Health - 1].rectTransform.DORewind();
        m_healthBackgroundImages[m_health.Health - 1].rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.shortSharp);
    }

    // Moves the player in a given direction
    public void Move(Vector2 _direction)
    {
        // Only rotate and move character if there is directional input
        if (_direction.magnitude != 0.0f)
        {
            // Change the move direction relative to the camera
            Vector3 moveDir = Camera.main.RelativeDirection2(_direction);

            // Set look direction
            transform.forward = moveDir;

            // Set move force
            float force = (m_inSand) ? m_settings.m_sandMoveForce : m_settings.m_moveForce;

            // Add force
            m_rigidBody.AddForce(moveDir * force, ForceMode.Impulse);
        }

        ApplyDrag();
    }

    // Applies drag and gravity to the player
    private void ApplyDrag()
    {
        // Drag
        Vector3 vel = m_rigidBody.velocity;
        vel.x *= (0.98f / m_settings.m_drag);
        vel.z *= (0.98f / m_settings.m_drag);
        m_rigidBody.velocity = vel;

        // Gravity
        m_rigidBody.AddForce(Vector3.up * m_settings.m_gravity);
    }

    public void CancelStairsGravity(Vector3 _right)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            m_rigidBody.AddForce(Vector3.down * m_settings.m_gravity);
            m_rigidBody.AddForce(hit.normal * m_settings.m_gravity);
        }
    }

    // Makes the player punch
    public void Punch(EChunkEffect _effect)
    {
        Vector3 spawnPos = transform.position;

        spawnPos += (transform.forward.normalized * m_settings.m_hurtboxMoveBy);

        // 
        spawnPos.y += GetComponent<Collider>().bounds.size.y * 0.5f;

        // Spawn in
        Hurtbox hurtbox = Instantiate(m_hurtboxPrefab, spawnPos, transform.rotation).GetComponent<Hurtbox>();
        hurtbox.Init(transform.position, _effect);
    }

    public bool TryConfirmChunk()
    {
        Tile tile = m_tileTargeter.GetClosest();

        if (tile)
        {
            bool tileFree = !tile.IsOccupied();

            if (tileFree) { m_confirmedTile = tile; }
            else { m_confirmedTile = null; }

            return tileFree;
        }
        else
        {
            return false;
        }
    }

    // Raises a given chunk
    public void RaiseChunk()
    {
        Debug.Assert(m_confirmedTile, "Confirmed tile was null");

        Chunk newChunk = m_confirmedTile.TryRaiseChunk();

        m_confirmedTile = null;
    }

    public void KnockBack(Vector3 _dir)
    {
        m_rigidBody.AddForce(_dir * m_settings.m_knockbackForce, ForceMode.Impulse);
    }

    public void Interact()
    {
        Interactable.s_closest.Invoke();
    }

    // Debug - remove on build
    private void Update()
    {
        // Toggles invincibility
        if (Input.GetKeyDown(KeyCode.T))
        {
            m_health.IsInvincible = !m_health.IsInvincible;
            Debug.Log("Invincibility set to: " + m_health.IsInvincible);
        }
    }

    // Pauses the game
    public void Pause()
    {
        Time.timeScale = 0.0f;
        HitFreezeManager.s_ogTimeScale = 0.0f;
        AudioManager.Instance.PauseAll();
        m_input.SetPause(true);

        // Remove event systems
        m_eventSystems = FindObjectsOfType<EventSystem>();
        for (int i = 0; i < m_eventSystems.Length; i++)
        {
            m_eventSystems[i].enabled = false; 
        }

        m_pauseMenu = Instantiate(m_pauseMenuPrefab, Vector3.zero, Quaternion.identity);
    }

    // UnPauses the game
    public void UnPause()
    {
        if (!HitFreezeManager.s_frozen)
        {
            Time.timeScale = 1.0f;
        }
        HitFreezeManager.s_ogTimeScale = 1.0f;
        m_input.SetPause(false);
        AudioManager.Instance.ContinuePlay();
        Destroy(m_pauseMenu);

        // Reenable event systems
        for (int i = 0; i < m_eventSystems.Length; i++)
        {
            m_eventSystems[i].enabled = true; 
        }
    }

    public void ContinueDialogue()
    {
        MessageBus.TriggerEvent(EMessageType.continueDialogue);
    }

    private void SetOutlineColour()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        int outlineColourID = Shader.PropertyToID("_SilhouetteColor");

        SceneOutlineColour outline;
        outline = m_settings.m_outlineColours.Find(t => t.sceneName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        if (outline != null)
        {
            Debug.Log("Setting mpb");
            mpb.SetColor(outlineColourID, outline.outlineColor);
            m_meshRenderer.SetPropertyBlock(mpb);
        }
        else
        {
            Debug.Log("Couldn't set mpb");
        }
    }

    public void UpdateHealthSprites()
    {
        int newCurrentHealth = m_health.Health;

        for (int i = 0; i < 6; i++)
        {
            if (i < newCurrentHealth)
            {
                m_healthImages[i].enabled = true;
            }
            else
            {
                m_healthImages[i].enabled = false;
            }
        }
    }

    public void SetMaxHealth(int _newMax)
    {
        _newMax = Mathf.Clamp(_newMax, 1, 6);

        // Activate / disable background images
        for (int i = 0; i < 6; i++)
        {
            if (i < _newMax)
            {
                m_healthBackgroundImages[i].enabled = true;
            }
            else
            {
                m_healthBackgroundImages[i].enabled = false;
            }
        }

        if (!m_health) { m_health = GetComponent<HealthComponent>(); }
        m_health.SetMaxHealth(_newMax);

        UpdateHealthSprites();
    }
    public int GetCurrentHealth()
    {
        return m_health.Health; 
    }
}
