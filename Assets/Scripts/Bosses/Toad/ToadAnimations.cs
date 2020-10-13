using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadAnimations : MonoBehaviour
{
    public ToadBoss m_toadBoss;
    public ToadSpit m_spitAttack;
    public ToadSwell m_swell;
    public ToadSwampAttack m_swampAttack;
    public ToadTongueAttack m_tongueAttack;

    public void AESpitProjectile() => m_spitAttack.AESpitProjectile();
    public void AELaunchWave()
    {
        m_swampAttack.AELaunchWave();
    }
    public void AEFrogLand()
    {
        MessageBus.TriggerEvent(EMessageType.toadLand);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);
    }

    public void AESwampComplete() => m_swampAttack.AEBehaviourComplete();
    public void AETongueComplete() => m_tongueAttack.AEBehaviourComplete();
    public void AESpitComplete() => m_spitAttack.AEBehaviourComplete();
    public void AESwellComplete() => m_swell.AEBehaviourComplete();
    public void AEExtendTongue() => m_tongueAttack.AEExtendTongue();
    public void AESwallow() => m_tongueAttack.AESwallow();
    public void AEAwaken() => m_toadBoss.AEAwaken();
    public void AEPlayRoarSound()
    {
        MessageBus.TriggerEvent(EMessageType.toadRoar);
        ScreenshakeManager.Shake(ScreenshakeManager.EShakeType.medium);
    }

    public void AEBigSplash() => MessageBus.TriggerEvent(EMessageType.toadJumpInWater);
    public void AESmallSplash() => MessageBus.TriggerEvent(EMessageType.smallToadJumpInWater);
    public void AEOnDeath() => m_toadBoss.ActivateCrystal();

}
