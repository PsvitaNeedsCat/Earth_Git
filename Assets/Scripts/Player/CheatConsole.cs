using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheatConsole : MonoBehaviour
{
    bool m_showConsole = false;

    string m_input = "";

    public static CheatCommand AAAA;
    public static CheatCommand OCEAN_MAN;
    public static CheatCommand<int> SET_CUR_HEALTH;
    public static CheatCommand<int> SET_MAX_HEALTH;
    public static CheatCommand<EChunkEffect> TOGGLE_POWER;

    public List<object> m_commandList;

    private PlayerController m_playerController;
    private Player m_player;

    public void OnReturn()
    {
        if (m_showConsole)
        {
            HandleInput();
            m_input = "";
        }
    }

    public void OnToggleDebug()
    {
        m_showConsole = !m_showConsole;
    }

    private void Awake()
    {
        m_playerController = GetComponent<PlayerController>();
        m_player = GetComponent<Player>();

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

        m_commandList = new List<object>
        {
            AAAA,
            OCEAN_MAN,
            SET_CUR_HEALTH,
            SET_MAX_HEALTH,
            TOGGLE_POWER
        };
    }

    private void OnGUI()
    {
        if (!m_showConsole)
        {
            return;
        }

        float consoleHeight = 0.0f;

        GUI.Box(new Rect(0, consoleHeight, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        m_input = GUI.TextField(new Rect(10.0f, consoleHeight + 5.0f, Screen.width - 20.0f, 20.0f), m_input);
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
