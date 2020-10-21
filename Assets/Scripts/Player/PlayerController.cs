using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    // Public variables
    [HideInInspector] public Tile m_confirmedTile = null;
    [HideInInspector] public bool m_inSand = false;
    [HideInInspector] public bool m_firstPerson = false;
    [HideInInspector] public Rigidbody m_rigidBody;
    [HideInInspector] public static bool s_saveOnAwake = false;
    public SkinnedMeshRenderer m_meshRenderer;

    // Serialized Variables
    [SerializeField] private GameObject m_hurtboxPrefab;
    [SerializeField] private TileTargeter m_tileTargeter;
    [SerializeField] private GameObject m_moustache;
    private List<Image> m_healthImages;
    private List<Image> m_healthBackgroundImages;
    [SerializeField] private AnimationCurve m_movementCurve = new AnimationCurve();

    // Private variables
    private PlayerController m_instance;
    private GlobalPlayerSettings m_settings;
    private HealthComponent m_health;
    private PlayerInput m_input;
    private GameObject m_pauseMenu = null;
    private GameObject m_pauseMenuPrefab;
    private EventSystem[] m_eventSystems;
    private GameCanvas m_gameCanvas;
    private PlayerRagdoll m_ragdoll;
    private CheatConsole m_cheats;
    private float m_defaultVignette = 0.0f;

    private Sequence m_healthAnim;
    private Sequence m_healthBGAnim;

    private void Awake()
    {
        // Only one instance
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("A second instance of PlayerController.cs was instantiated");
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }

        m_gameCanvas = FindObjectOfType<GameCanvas>();
        m_healthImages = m_gameCanvas.m_healthImages;
        m_healthBackgroundImages = m_gameCanvas.m_healthBackgroundImages;

        m_settings = Resources.Load<GlobalPlayerSettings>("ScriptableObjects/GlobalPlayerSettings");
        m_ragdoll = GetComponent<PlayerRagdoll>();
        m_cheats = GetComponent<CheatConsole>();

        SetOutlineColour();

        // Set health
        m_health = GetComponent<HealthComponent>();

        // Set rigidbody
        m_rigidBody = GetComponent<Rigidbody>();

        m_input = GetComponent<PlayerInput>();
        Debug.Assert(m_input, "Player has no PlayerInput.cs");

        m_pauseMenuPrefab = Resources.Load<GameObject>("Prefabs/Pause Menu Parent");

        // Save
        if (s_saveOnAwake)
        {
            if (SaveManager.Instance)
            {
                SaveManager.Instance.SaveGame();
            }
            s_saveOnAwake = false;
        }
    }

    private void Start()
    {
        // Get default vignette
        PostProcessVolume pp = FindObjectOfType<PostProcessVolume>();
        if (pp)
        {
            Vignette v;
            pp.profile.TryGetSettings(out v);
            if (v)
            {
                m_defaultVignette = v.intensity.value;
            }
        }

        m_healthAnim = DOTween.Sequence();
        m_healthBGAnim = DOTween.Sequence();

        // Set max health based on powers unlocked
        int maxHealth = 2;
        foreach (KeyValuePair<EChunkEffect, bool> i in Player.s_activePowers)
        {
            if (i.Value)
            {
                ++maxHealth;
            }
        }
        m_health.Init(maxHealth, maxHealth, OnHurt, OnHealed, OnDeath);
        SetMaxHealth(maxHealth);
        UpdateHealthSprites();

        // Set init spawn location
        if (RoomManager.Instance)
        {
            RoomManager.Instance.m_respawnLocation = transform.position;
        }
    }

    private void OnEnable()
    {
        MessageBus.AddListener(EMessageType.fadedToBlackQuiet, AfterDeath);
    }
    private void OnDisable()
    {
        MessageBus.RemoveListener(EMessageType.fadedToBlackQuiet, AfterDeath);
    }

    private void OnHurt()
    {
        UpdateHealthSprites();

        MessageBus.TriggerEvent(EMessageType.playerHurt);

        m_health.SetInvincibleTimer(m_settings.m_hurtTime);

        // Tween colour change

        Material mat = m_meshRenderer.material;
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => mat.GetFloat("_WhiteOverride"), x => mat.SetFloat("_WhiteOverride", x), 1.0f, m_settings.m_hurtTime / 12.0f));
        seq.Append(DOTween.To(() => mat.GetFloat("_WhiteOverride"), x => mat.SetFloat("_WhiteOverride", x), 0.0f, m_settings.m_hurtTime / 12.0f));
        seq.SetLoops(2);
        seq.Play();

        // Tween health bar
        m_healthBackgroundImages[m_health.Health].rectTransform.DORewind();
        m_healthBackgroundImages[m_health.Health].rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.shortSharp);

        // Vignette
        if (m_health.Health == 1)
        {
            PostProcessVolume pp = FindObjectOfType<PostProcessVolume>();
            if (pp)
            {
                Vignette v;
                pp.profile.TryGetSettings(out v);
                if (v)
                {
                    v.intensity.value = m_defaultVignette + 0.2f;
                }
            }
        }
    }

    private void OnDeath()
    {
        // Freeze player
        m_input.SetCombat(false);
        m_input.SetMovement(false);
        if (m_ragdoll)
        {
            m_ragdoll.SetRagdoll(true);
        }

        // Fade to black
        RoomManager.Instance.FadeToBlack();
        MessageBus.TriggerEvent(EMessageType.playerDeath);
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
        if (m_ragdoll)
        {
            m_ragdoll.SetRagdoll(false);
        }
    }

    private void OnHealed()
    {
        m_healthAnim.Kill();
        m_healthBGAnim.Kill();
        m_healthAnim = DOTween.Sequence();
        m_healthBGAnim = DOTween.Sequence();

        for (int i = 0; i < 6; i++)
        {
            // Health
            m_healthImages[i].rectTransform.DORewind();
            if (i >= m_health.Health)
            {
                m_healthImages[i].enabled = false;
            }
            else
            {
                if (m_healthImages[i].enabled)
                {
                    // Pulse
                    m_healthImages[i].rectTransform.transform.localScale = Vector3.one;
                    m_healthAnim.Insert(i * 0.25f, m_healthImages[i].rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.5f));
                }
                else
                {
                    // Scale
                    m_healthImages[i].enabled = true;
                    m_healthImages[i].rectTransform.transform.localScale = Vector3.zero;
                    m_healthAnim.Insert(i * 0.25f, m_healthImages[i].rectTransform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBounce));
                }
            }

            // Background
            m_healthBackgroundImages[i].rectTransform.DORewind();
            if (!m_healthBackgroundImages[i].enabled && i < m_health.m_maxHealth)
            {
                m_healthBackgroundImages[i].enabled = true;
                m_healthBackgroundImages[i].rectTransform.transform.localScale = Vector3.zero;
                m_healthBGAnim.Insert(i * 0.25f, m_healthBackgroundImages[i].rectTransform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBounce));
            }
        }

        // Vignette
        if (m_health.Health > 1)
        {
            PostProcessVolume pp = FindObjectOfType<PostProcessVolume>();
            if (pp)
            {
                Vignette v;
                pp.profile.TryGetSettings(out v);
                if (v)
                {
                    v.intensity.value = m_defaultVignette;
                }
            }
        }

        m_healthAnim.Play();
        m_healthBGAnim.Play();

        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.shortSharp);
    }

    // Moves the player in a given direction
    public void Move(Vector2 _direction)
    {
        if (!m_firstPerson)
        {
            // Only rotate and move character if there is directional input
            if (_direction.magnitude != 0.0f)
            {
                // Change the move direction relative to the camera
                Vector3 moveDir = Camera.main.RelativeDirection2(_direction);

                // Set look direction
                transform.forward = moveDir;

            float speed = m_movementCurve.Evaluate(_direction.magnitude);

            // Set move force
            float force = (m_inSand) ? m_settings.m_sandMoveForce : m_settings.m_moveForce;
            force *= speed;

                // Add force
                m_rigidBody.AddForce(moveDir * force, ForceMode.Impulse);
            }
        }
        // First person
        else
        {
            if (_direction.magnitude != 0.0f)
            {
                transform.Rotate(0.0f, _direction.x * 3.0f, 0.0f);

                float speed = m_movementCurve.Evaluate(_direction.magnitude);

                // Set move force
                float force = (m_inSand) ? m_settings.m_sandMoveForce : m_settings.m_moveForce;

                // Add force
                m_rigidBody.AddForce(transform.forward * _direction.y * force, ForceMode.Impulse);
            }
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

    // Sets the moustache's scale and turns it off if 0
    public void SetMoustacheScale(float _scale)
    {
        if (_scale != 0.0f)
        {
            m_moustache.SetActive(true);
            m_moustache.transform.DOScale(Vector3.one * _scale, 0.5f).SetEase(Ease.InElastic);
        }
        else
        {
            m_moustache.transform.DOScale(Vector3.one * _scale, 0.5f).SetEase(Ease.InElastic).OnComplete(() => m_moustache.SetActive(false));
        }
    }

    public void Jump()
    {
        if (m_cheats.ConsoleOpen())
        {
            return;
        }

        m_rigidBody.AddForce(Vector3.up * 15.0f, ForceMode.Impulse);
    }

    public void ToggleInvincibility()
    {
        m_health.IsInvincible = !m_health.IsInvincible;
        Debug.Log("Invincibility set to: " + m_health.IsInvincible);
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
        if (m_eventSystems != null)
        {
            for (int i = 0; i < m_eventSystems.Length; i++)
            {
                m_eventSystems[i].enabled = true;
            }
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
            // Health
            m_healthImages[i].enabled = i < newCurrentHealth;

            // Background
            m_healthBackgroundImages[i].enabled = i < m_health.m_maxHealth;
        }
    }

    public void SetMaxHealth(int _newMax)
    {
        if (!m_health) 
        {
            m_health = GetComponent<HealthComponent>(); 
        }
        m_health.SetMaxHealth(_newMax);
    }

    public void SetCurrentHealth(int _newValue)
    {
        m_health.Health = _newValue;
        UpdateHealthSprites();
    }

    public int GetCurrentHealth()
    {
        return m_health.Health; 
    }
}
