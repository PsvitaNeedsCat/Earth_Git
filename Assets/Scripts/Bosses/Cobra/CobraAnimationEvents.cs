using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraAnimationEvents : MonoBehaviour
{
    public CobraMirageSpit m_mirageSpit;
    public CobraAnimations m_animations;

    public void AECobraFire()
    {
        m_mirageSpit.FireProjectile();
    }

    public void AEFlipTiles()
    {
        m_animations.AEFlipTiles();
    }
}
