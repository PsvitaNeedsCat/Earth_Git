using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraAnimationEvents : MonoBehaviour
{
    public CobraMirageSpit m_mirageSpit;

    public void AECobraFire()
    {
        m_mirageSpit.FireProjectile();
    }
}
