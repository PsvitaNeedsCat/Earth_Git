using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public class OneSecondFlipNode : LeafNode
    {
        float m_timer = 1.0f;
        bool m_flipped = false;
        bool m_result;

        public OneSecondFlipNode()
        {
            m_actionDelegate = FlipDelegate;
        }

        ENodeState FlipDelegate()
        {
            m_timer -= Time.deltaTime;

            if (m_timer < 0.0f)
            {
                int flip = Random.Range(0, 2);
                m_result = (flip == 0) ? false : true;
                Debug.Log("Flip result: " + m_result.ToString());
                m_flipped = true;
            }

            if (m_flipped)
            {
                return (m_result) ? ENodeState.Success : ENodeState.Failure;
            }

            return ENodeState.Running;
        }
    }
}