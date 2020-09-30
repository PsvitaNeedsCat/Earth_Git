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

    public static CheatCommand AAAA;
    public static CheatCommand OCEAN_MAN;
    public static CheatCommand<int> SET_CUR_HEALTH;
    public static CheatCommand<int> SET_MAX_HEALTH;
    public static CheatCommand<EChunkEffect> TOGGLE_POWER;
    public static CheatCommand HELP;

    public List<object> m_commandList;

    private PlayerController m_playerController;
    private Player m_player;
    private PlayerInput m_playerInput;

    private bool m_justOpened = false;

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
        m_showConsole = !m_showConsole;

        if (m_showConsole)
        {
            m_justOpened = true;
            m_input = "";
        }

        m_playerInput.SetMovement(!m_showConsole);
        m_playerInput.SetCombat(!m_showConsole);
    }

    private void Awake()
    {
        m_playerController = GetComponent<PlayerController>();
        m_player = GetComponent<Player>();
        m_playerInput = GetComponent<PlayerInput>();

        AAAA = new CheatCommand("aaaa", "A hunk of hunk of burnin' love", "aaaa", () =>
        {
            MessageBus.TriggerEvent(EMessageType.aaaa);
        });

        OCEAN_MAN = new CheatCommand("ocean_man", "Take me by the hand", "ocean_man", () =>
        {
            MessageBus.TriggerEvent(EMessageType.oceanMan);
        });

        SET_CUR_HEALTH = new CheatCommand<int>("set_cur_health", "Sets the player's current health", "set_cur_health <health_amount>", (x) =>
        {
            m_playerController.SetCurrentHealth(x);
        });

        SET_MAX_HEALTH = new CheatCommand<int>("set_max_health", "Sets the player's max health", "set_max_health <new_max>", (x) =>
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
        });

        m_commandList = new List<object>
        {
            AAAA,
            OCEAN_MAN,
            SET_CUR_HEALTH,
            SET_MAX_HEALTH,
            TOGGLE_POWER,
            HELP    
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

        // On opened
        if (m_justOpened)
        {
            m_justOpened = false;

            GUI.FocusControl("ConsoleInput");
        }
    }

    private void HandleInput()
    {
        string[] properties = m_input.Split(' ');

        for (int i = 0; i < m_commandList.Count; i++)
        {
            CheatCommandBase commandBase = m_commandList[i] as CheatCommandBase;

            if (m_input.Contains(commandBase.m_commandId))
            {
                CheatCommand command = (m_commandList[i] as CheatCommand);
                CheatCommand<int> intCommand = (m_commandList[i] as CheatCommand<int>);
                CheatCommand<EChunkEffect> effectCommand = (m_commandList[i] as CheatCommand<EChunkEffect>);
                if (command != null)
                {
                    command.Invoke();
                }
                else if (intCommand != null)
                {
                    intCommand.Invoke(int.Parse(properties[1]));
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
