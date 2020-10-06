using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLavaTrail : Lava
{
    protected override void Awake()
    {
        base.Awake();

        // Destroy after lifetime is up
        StartCoroutine(DestroyAfterSeconds());
    }

    protected override void TurnToStone()
    {
        base.TurnToStone();

        MessageBus.TriggerEvent(EMessageType.lavaToStone);

        Destroy(transform.parent.gameObject);
    }

    // Destroys the lava trail after the lifetime is over
    private IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(CentipedeBoss.s_settings.m_lavaLifetime);

        // Wait to destroy tile
        while (m_tweeningChunk)
        {
            yield return null;
        }

        Destroy(transform.parent.gameObject);
    }
}
