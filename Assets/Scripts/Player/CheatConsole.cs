using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheatConsole : MonoBehaviour
{
    private bool m_showConsole = false;
    private bool m_showHelp = false;
    private Vector2 m_scroll;

    string m_input = "";

    public static CheatCommand<int> CUR_HEALTH;
    public static CheatCommand<int> MAX_HEALTH;
    public static CheatCommand<EChunkEffect> TOGGLE_POWER;
    public static CheatCommand HELP;
    public static CheatCommand<float> TIME_SCALE;
    public static CheatCommand DEV_MODE;
    public static CheatCommand DEV_MODE_FAST;
    public static CheatCommand GOD_MODE;
    public static CheatCommand JUMP;
    public static CheatCommand<string> PLAY_SOUND;
    public static CheatCommand KILL_PLAYER;
    public static CheatCommand<float> SET_MOUSTACHE;
    public static CheatCommand TOGGLE_JUMP;
    public static CheatCommand ERROR;

    public List<object> m_commandList;

    private PlayerController m_playerController;
    private Player m_player;
    private PlayerInput m_playerInput;

    private bool m_justOpened = false;
    private List<string> m_previousEntries = new List<string>();
    private int m_entryIndex = 0;
    private bool m_justRetrievedEntry = false;

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
            m_playerController.SetCurrentHealth(6);
            m_player.TogglePower(EChunkEffect.water);
            m_player.TogglePower(EChunkEffect.fire);
            m_player.TogglePower(EChunkEffect.mirage);
            m_playerController.ToggleInvincibility();
            m_playerInput.ToggleJump();
        });

        DEV_MODE_FAST = new CheatCommand("dev_mode_fast", "Max health, all powers, invincibility, speed up time", "dev_mode_fast", () =>
        {
            m_playerController.SetMaxHealth(6);
            m_playerController.SetCurrentHealth(6);
            m_player.TogglePower(EChunkEffect.water);
            m_player.TogglePower(EChunkEffect.fire);
            m_player.TogglePower(EChunkEffect.mirage);
            m_playerController.ToggleInvincibility();
            m_playerInput.ToggleJump();
            Time.timeScale = 3.0f;
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
            TOGGLE_JUMP,
            ERROR,
        };
    }

    private void OnGUI()
    {
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
                if (command != null)
                {
                    command.Invoke();
                }
                else if (intCommand != null)
                {
                    intCommand.Invoke(int.Parse(properties[1]));
                }
                else if (floatCommand != null)
                {
                    floatCommand.Invoke(float.Parse(properties[1]));
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

            }
        }
    }
}
