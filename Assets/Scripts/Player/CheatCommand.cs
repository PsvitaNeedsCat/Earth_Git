using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheatCommandBase
{
    public string m_commandId;
    public string m_commandDescription;
    public string m_commandFormat;

    public CheatCommandBase(string _id, string _description, string _format)
    {
        m_commandDescription = _id;
        m_commandDescription = _description;
        m_commandFormat = _format;
    }
}


public class CheatCommand : CheatCommandBase
{
    private Action m_command;

    public CheatCommand(string _id, string _description, string _format, Action _command) : base (_id, _description, _format)
    {
        this.m_command = _command;
    }

    public void Invoke()
    {
        m_command.Invoke();
    }
}