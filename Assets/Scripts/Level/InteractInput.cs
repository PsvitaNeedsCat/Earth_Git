using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractInput : MonoBehaviour
{
    private InputMaster m_input = null;

    private void Awake()
    {
        m_input = new InputMaster();

        m_input.Dialogue.Continue.performed += _ => Interact();
        m_input.Dialogue.Enable();
    }

    private void Interact()
    {
        MessageBus.TriggerEvent(EMessageType.continueDialogue);
    }
}
