using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackWallAnimator : MonoBehaviour
{
    public void AEChangeRoom()
    {
        RoomManager.Instance.ChangeRooms();
    }
}
