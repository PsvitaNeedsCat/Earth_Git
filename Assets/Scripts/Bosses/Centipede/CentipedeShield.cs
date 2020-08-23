using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeShield : MonoBehaviour
{
    public void Break()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
