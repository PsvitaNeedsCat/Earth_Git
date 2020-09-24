using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAutoSave : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            SaveManager sm = FindObjectOfType<SaveManager>();
            if (sm)
            {
                sm.SaveGame();
            }
        }
    }
}
