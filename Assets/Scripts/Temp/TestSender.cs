using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSender : MonoBehaviour
{
    public void TestMessage()
    {
        MessageBus.TriggerEvent(EMessageType.PlayerPunch);
    }

    public void TestMessage2()
    {
        MessageBus.TriggerEvent(EMessageType.PlayerPunch);
    }
}
