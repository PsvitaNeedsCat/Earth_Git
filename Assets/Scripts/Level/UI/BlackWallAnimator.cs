using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackWallAnimator : MonoBehaviour
{
    public void AEChangeRoom()
    {
        MessageBus.TriggerEvent(EMessageType.fadedToBlack);
    }

    // When faded to black doesn't change rooms
    public void AEQuietFaded()
    {
        MessageBus.TriggerEvent(EMessageType.fadedToBlackQuiet);
    }
}
