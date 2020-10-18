using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class CheatConsole : MonoBehaviour
{
    public GameObject m_headObject;
    public GameObject m_leftArmObject;
    public GameObject m_rightArmObject;
    public GameObject m_leftLegObject;
    public GameObject m_rightLegObject;
    public GameObject m_playerMesh;

    private bool m_showConsole = false;
    private bool m_showHelp = false;
    private bool m_showTimeScale = false;
    private bool m_showFPS = false;
    private float m_deltaTime = 0.0f;
    private GUIStyle m_textStyle;
    private Vector2 m_scroll;

    string m_input = "";

    public static CheatCommand<int> CUR_HEALTH;
    public static CheatCommand<int> MAX_HEALTH;
    public static CheatCommand<EChunkEffect> TOGGLE_POWER;
    public static CheatCommand HELP;
    public static CheatCommand<float> TIME_SCALE;
    public static CheatCommand DEV_MODE;
    public static CheatCommand DEV_MODE_FAST;
    public static CheatCommand DEV_MODE_FAT;
    public static CheatCommand GOD_MODE;
    public static CheatCommand JUMP;
    public static CheatCommand<string> PLAY_SOUND;
    public static CheatCommand KILL_PLAYER;
    public static CheatCommand<float> SET_MOUSTACHE;
    public static CheatCommand TOGGLE_JUMP;
    public static CheatCommand ERROR;
    public static CheatCommand FIRST_PERSON;
    public static CheatCommand<float> HEAD_SIZE;
    public static CheatCommand<float> ARMS_SIZE;
    public static CheatCommand<float> LEGS_SIZE;
    public static CheatCommand<float> PLAYER_SIZE;
    public static CheatCommand<Vector3> HEAD_SCALE;
    public static CheatCommand<Vector3> ARMS_SCALE;
    public static CheatCommand<Vector3> LEGS_SCALE;
    public static CheatCommand<Vector3> PLAYER_SCALE;
    public static CheatCommand<int> ROOM;
    public static CheatCommand NEXT_ROOM;
    public static CheatCommand PREV_ROOM;
    public static CheatCommand CENTIPEDE_CAM;
    public static CheatCommand TOP_DOWN;
    public static CheatCommand CAMERA_PROJECTION;
    public static CheatCommand<EHatType> HAT;
    public static CheatCommand SHOW_TIME_SCALE;
    public static CheatCommand SHOW_FPS;

    public List<object> m_commandList;

    private PlayerController m_playerController;
    private Player m_player;
    private PlayerInput m_playerInput;
    private PlayerHats m_playerHats;

    private bool m_justOpened = false;
    private List<string> m_previousEntries = new List<string>();
    private int m_entryIndex = 0;
    private bool m_justRetrievedEntry = false;

    private Quaternion m_originalCamera;
    private Quaternion m_topDownCamera = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
    private bool m_topDownActive = false;

    public bool ConsoleOpen()
    {
        return m_showConsole;
    }

    public void OnReturn()
    {
        if (m_showConsole)
        {
            HandleInput();
            m_input = "";

            OnToggleDebug();
        }
    }

    public void OnToggleDebug()
    {
        m_entryIndex = m_previousEntries.Count;

        m_showConsole = !m_showConsole;

        if (m_showConsole)
        {
            m_justOpened = true;
            m_input = "";
        }

        m_playerInput.SetMovement(!m_showConsole);
        m_playerInput.SetCombat(!m_showConsole);
    }

    public void PreviousEntry()
    {
        if (!ConsoleOpen() || m_entryIndex - 1 < 0)
        {
            return;
        }

        --m_entryIndex;
        m_input = m_previousEntries[m_entryIndex];

        m_justRetrievedEntry = true;
    }

    public void NextEntry()
    {
        if (!ConsoleOpen() || m_entryIndex + 1 >= m_previousEntries.Count)
        {
            return;
        }

        ++m_entryIndex;
        m_input = m_previousEntries[m_entryIndex];

        m_justRetrievedEntry = true;
    }

    private void Awake()
    {
        m_playerController = GetComponent<PlayerController>();
        m_player = GetComponent<Player>();
        m_playerInput = GetComponent<PlayerInput>();
        m_playerHats = GetComponent<PlayerHats>();

        m_textStyle = new GUIStyle();
        m_textStyle.fontSize = 18;
        m_textStyle.normal.textColor = Color.white;

        CUR_HEALTH = new CheatCommand<int>("cur_health", "Sets the player's current health", "cur_health <health_amount>", (x) =>
        {
            m_playerController.SetCurrentHealth(x);
        });

        MAX_HEALTH = new CheatCommand<int>("max_health", "Sets the player's max health", "max_health <new_max>", (x) =>
        {
            m_playerController.SetMaxHealth(x);
        });

        TOGGLE_POWER = new CheatCommand<EChunkEffect>("toggle_power", "Toggles whether a power is unlocked", "toggle_power <effect>", (x) =>
        {
            m_player.TogglePower(x);
        });

        HELP = new CheatCommand("help", "Shows a list of commands", "help", () =>
        {
            m_showHelp = !m_showHelp;

            if (m_showHelp)
            {
                OnToggleDebug();
            }
        });

        TIME_SCALE = new CheatCommand<float>("time_scale", "Sets the game's time scale", "time_scale <scale>", (x) =>
        {
            Time.timeScale = x;
        });

        DEV_MODE = new CheatCommand("dev_mode", "Max health, all powers, invincibility", "dev_mode", () =>
        {
            m_playerController.SetMaxHealth(6);
            m_player.TogglePower(EChunkEffect.water);
            m_player.TogglePower(EChunkEffect.fire);
            m_player.TogglePower(EChunkEffect.mirage);
            m_playerController.ToggleInvincibility();
            m_playerInput.ToggleJump();
        });

        DEV_MODE_FAST = new CheatCommand("dev_mode_fast", "Max health, all powers, invincibility, speed up time", "dev_mode_fast", () =>
        {
            m_playerController.SetMaxHealth(6);
            m_player.TogglePower(EChunkEffect.water);
            m_player.TogglePower(EChunkEffect.fire);
            m_player.TogglePower(EChunkEffect.mirage);
            m_playerController.ToggleInvincibility();
            m_playerInput.ToggleJump();
            Time.timeScale = 3.0f;
        });

        DEV_MODE_FAT = new CheatCommand("dev_mode_fat", ":)", "dev_mode_fat", () =>
        {
            m_playerController.SetMaxHealth(6);
            m_player.TogglePower(EChunkEffect.water);
            m_player.TogglePower(EChunkEffect.fire);
            m_player.TogglePower(EChunkEffect.mirage);
            m_playerController.ToggleInvincibility();
            m_playerInput.ToggleJump();
            m_playerController.m_meshRenderer.transform.parent.localScale = new Vector3(3.0f, 1.0f, 3.0f);
        });

        GOD_MODE = new CheatCommand("god_mode", "Toggles invincibility", "god_mode", () =>
        {
            m_playerController.ToggleInvincibility();
        });

        JUMP = new CheatCommand("jump", "Toggles jump ability", "jump", () =>
        {
            m_playerInput.ToggleJump();
        });

        PLAY_SOUND = new CheatCommand<string>("play_sound", "Plays a sound effect", "play_sound <sound name>", (x) =>
        {
            EMessageType soundType;
            if (System.Enum.TryParse(x, out soundType))
            {
                MessageBus.TriggerEvent(soundType);
            }
        });

        KILL_PLAYER = new CheatCommand("kill_player", "Instantly kills the player", "kill_player", () =>
        {
            m_player.GetComponent<HealthComponent>().ForceKill();
        });

        SET_MOUSTACHE = new CheatCommand<float>("set_moustache", "Sets the moustache's scale", "set_moustache <scale value>", (x) =>
        {
            m_playerController.SetMoustacheScale(x);
        });

        TOGGLE_JUMP = new CheatCommand("toggle_jump", "Toggles the ability to jump. (Press C)", "toggle_jump", () =>
        {
            m_playerInput.ToggleJump();
        });

        ERROR = new CheatCommand("error", "Throws a null reference error.", "error", () =>
        {
            GameObject errorObj = GameObject.Find("ObjectThatIsn'tReal");
            errorObj.SetActive(true);
        });

        FIRST_PERSON = new CheatCommand("first_person", "Toggles first-person camera", "first_person", () =>
        {
            m_player.ToggleFirstPerson();
        });

        HEAD_SIZE = new CheatCommand<float>("head_size", "Sets the scale of the player's head", "head_size <scale>", (x) =>
        {
            m_headObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
        });

        ARMS_SIZE = new CheatCommand<float>("arms_size", "Sets the scale of the player's arms", "arms_size <scale>", (x) =>
        {
            m_leftArmObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
            m_rightArmObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
        });

        LEGS_SIZE = new CheatCommand<float>("legs_size", "Sets the scale of the player's legs", "legs_size <scale>", (x) =>
        {
            m_leftLegObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
            m_rightLegObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
        });

        PLAYER_SIZE = new CheatCommand<float>("player_size", "Sets the size of the whole player", "player_size <scale>", (x) =>
        {
            m_playerMesh.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
        });

        HEAD_SCALE = new CheatCommand<Vector3>("head_scale", "Sets the scale of the player's head", "head_scale <x scale> <y scale> <z scale>", (x) =>
        {
            m_headObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
        });

        ARMS_SCALE = new CheatCommand<Vector3>("arms_scale", "Sets the scale of the player's arms", "arms_scale <x scale> <y scale> <z scale>", (x) =>
        {
            m_leftArmObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
            m_rightArmObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
        });

        LEGS_SCALE = new CheatCommand<Vector3>("legs_scale", "Sets the scale of the player's legs", "legs_scale <x scale> <y scale> <z scale>", (x) =>
        {
            m_leftLegObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
            m_rightLegObject.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
        });

        PLAYER_SCALE = new CheatCommand<Vector3>("player_scale", "Scales the whole player", "player_scale <x scale> <y scale> <z scale>", (x) =>
        {
            m_playerMesh.transform.DOScale(x, 0.5f).SetEase(Ease.InOutElastic);
        });

        ROOM = new CheatCommand<int>("room", "Teleports the player to the specified room", "room <room number>", (x) =>
        {
            RoomManager.Instance.ForceLoadRoom(x);
        });

        NEXT_ROOM = new CheatCommand("next_room", "Moves the player to the next room", "next_room", () =>
        {
            RoomManager.Instance.ForceLoadRoom(RoomManager.Instance.GetCurrentRoom() + 1);
        });

        PREV_ROOM = new CheatCommand("prev_room", "Moves the player to the previous room", "prev_room", () =>
        {
            RoomManager.Instance.ForceLoadRoom(RoomManager.Instance.GetCurrentRoom() - 1);
        });

        CENTIPEDE_CAM = new CheatCommand("centipede_cam", "Gives the player a camera attached to the centipede", "centipede_cam", () =>
        {
            CentipedeBoss centipede = FindObjectOfType<CentipedeBoss>();

            if (centipede)
            {
                centipede.ToggleCentipedeCam();
            }
        });

        TOP_DOWN = new CheatCommand("top_down", "Toggles a top-down camera", "top_down", () =>
        {
            GameObject vCamObject = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;

            if (m_topDownActive)
            {
                vCamObject.transform.rotation = m_originalCamera;
            }
            else
            {
                m_originalCamera = vCamObject.transform.rotation;
                vCamObject.transform.rotation = m_topDownCamera;
            }

            m_topDownActive = !m_topDownActive;
        });

        CAMERA_PROJECTION = new CheatCommand("camera_projection", "Toggles the camera mode", "camera_projection", () =>
        {
            Camera.main.orthographic = !Camera.main.orthographic;
        });
        
        HAT = new CheatCommand<EHatType>("hat", "Gives the player a hat", "hat", (x) =>
        {
            m_playerHats.SetHat(x);
        });

        SHOW_TIME_SCALE = new CheatCommand("show_time_scale", "Toggles showing time scale", "show_time_scale", () =>
        {
            m_showTimeScale = !m_showTimeScale;
        });

        SHOW_FPS = new CheatCommand("show_fps", "Toggles showing FPS", "show_fps", () =>
        {
            m_showFPS = !m_showFPS;
        });

        m_commandList = new List<object>
        {
            CUR_HEALTH,
            MAX_HEALTH,
            TOGGLE_POWER,
            HELP,
            TIME_SCALE,
            DEV_MODE,
            GOD_MODE,
            JUMP,
            PLAY_SOUND,
            KILL_PLAYER,
            SET_MOUSTACHE,
            DEV_MODE_FAST,
            DEV_MODE_FAT,
            TOGGLE_JUMP,
            ERROR,
            FIRST_PERSON,
            HEAD_SIZE,
            ARMS_SIZE,
            LEGS_SIZE,
            PLAYER_SIZE,
            HEAD_SCALE,
            ARMS_SCALE,
            LEGS_SCALE,
            PLAYER_SCALE,
            ROOM,
            NEXT_ROOM,
            PREV_ROOM,
            CENTIPEDE_CAM,
            TOP_DOWN,
            CAMERA_PROJECTION,
            HAT,
            SHOW_TIME_SCALE,
            SHOW_FPS
        };
    }

    private void OnGUI()
    {
        if (m_showTimeScale)
        {
            GUI.Label(new Rect(Screen.width - 350, 0, 200, 50), "Time scale: " + Time.timeScale.ToString(), m_textStyle);
        }

        if (m_showFPS)
        {
            float miliSeconds = m_deltaTime * 1000.0f;
            float fps = 1.0f / m_deltaTime;
            string fpsText = string.Format("{0:0.0} ms ({1:0.} fps)", miliSeconds, fps);
            GUI.Label(new Rect(Screen.width - 150, 0, 200, 50), fpsText, m_textStyle);
        }

        if (!m_showConsole)
        {
            return;
        }

        float consoleHeight = 0.0f;
        
        if (m_showHelp)
        {
            GUI.Box(new Rect(0, consoleHeight, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * m_commandList.Count);

            m_scroll = GUI.BeginScrollView(new Rect(0, consoleHeight + 5.0f, Screen.width, 90), m_scroll, viewport);

            for (int i = 0; i < m_commandList.Count; i++)
            {
                CheatCommandBase command = m_commandList[i] as CheatCommandBase;
                string label = $"{command.m_commandFormat} - {command.m_commandDescription}";
                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();
            consoleHeight += 100.0f;
        }        

        GUI.Box(new Rect(0, consoleHeight, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);

        GUI.SetNextControlName("ConsoleInput");
        m_input = GUI.TextField(new Rect(10.0f, consoleHeight + 5.0f, Screen.width - 20.0f, 20.0f), m_input);

        // User has just pressed Up or Down on the keyboard
        if (m_justRetrievedEntry)
        {
            TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            textEditor.MoveLineEnd();
            m_justRetrievedEntry = false;
        }

        // On opened
        if (m_justOpened)
        {
            m_justOpened = false;

            GUI.FocusControl("ConsoleInput");
        }
    }

    private void Update()
    {
        m_deltaTime += (Time.unscaledDeltaTime - m_deltaTime) * 0.1f;
    }

    private void HandleInput()
    {
        if (m_previousEntries.Count <= 0 || m_previousEntries[m_previousEntries.Count - 1] != m_input)
        {
            m_previousEntries.Add(m_input);
        }
        string[] properties = m_input.Split(' ');

        for (int i = 0; i < m_commandList.Count; i++)
        {
            CheatCommandBase commandBase = m_commandList[i] as CheatCommandBase;

            if (properties[0] == commandBase.m_commandId)
            {
                CheatCommand command = (m_commandList[i] as CheatCommand);
                CheatCommand<int> intCommand = (m_commandList[i] as CheatCommand<int>);
                CheatCommand<float> floatCommand = (m_commandList[i] as CheatCommand<float>);
                CheatCommand<string> stringCommand = (m_commandList[i] as CheatCommand<string>);
                CheatCommand<EChunkEffect> effectCommand = (m_commandList[i] as CheatCommand<EChunkEffect>);
                CheatCommand<Vector3> vectorThreeCommand = (m_commandList[i] as CheatCommand<Vector3>);
                CheatCommand<EHatType> hatCommand = (m_commandList[i] as CheatCommand<EHatType>);

                if (command != null)
                {
                    command.Invoke();
                }
                else if (intCommand != null)
                {
                    int parameter;
                    if (int.TryParse(properties[1], out parameter))
                    {
                        intCommand.Invoke(parameter);
                    }
                }
                else if (floatCommand != null)
                {
                    float parameter;
                    if (float.TryParse(properties[1], out parameter))
                    {
                        floatCommand.Invoke(parameter);
                    }
                }
                else if (stringCommand != null)
                {
                    stringCommand.Invoke(properties[1]);
                }
                else if (effectCommand != null)
                {
                    EChunkEffect effect;
                    if (System.Enum.TryParse(properties[1], out effect))
                    {
                        effectCommand.Invoke(effect);
                    }
                }
                else if (vectorThreeCommand != null)
                {
                    Vector3 vec;

                    if (float.TryParse(properties[1], out vec.x) && float.TryParse(properties[2], out vec.y) && float.TryParse(properties[3], out vec.z))
                    {
                        vectorThreeCommand.Invoke(vec);
                    }
                }
                else if (hatCommand != null)
                {
                    EHatType hatType;
                    if (System.Enum.TryParse(properties[1], out hatType))
                    {
                        hatCommand.Invoke(hatType);
                    }
                }

            }
        }
    }
}
