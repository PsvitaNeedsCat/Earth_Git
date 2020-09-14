using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string m_newSceneName = "";

    // Calls the RoomManager to change scenes
    private void OnTriggerEnter(Collider other)
    {
        PlayerInput player = other.GetComponent<PlayerInput>();
        if (player)
        {
            player.SetMovement(false);

            FindObjectOfType<RoomManager>().LoadScene(m_newSceneName);
        }
    }
}
