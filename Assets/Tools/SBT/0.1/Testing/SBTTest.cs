using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SBT
{
    public class SBTTest : MonoBehaviour
    {
        private BaseNode testOne;
        private BaseNode testTwo;
        private BaseNode testThree;
        private BaseNode testFour;
        bool m_selectorFinished = false;

        private void Start()
        {
            Random.InitState((int)System.DateTime.Now.Ticks);

            testOne = new OneSecondFlipNode();
            testTwo = new OneSecondFlipNode();
            testThree = new OneSecondFlipNode();
            testFour = new Selector(new List<BaseNode>
            {
                testOne,
                testTwo,
                testThree
            });
        }

        private void Update()
        {
            if (m_selectorFinished) return;

            ENodeState result = testFour.Process();
            
            if (result == ENodeState.Failure || result == ENodeState.Success)
            {
                m_selectorFinished = true;
            }
        }

        private ENodeState Failure()
        {
            return ENodeState.Failure;
        }

        private ENodeState Success()
        {
            return ENodeState.Success;
        }

        private ENodeState CoinFlip()
        {
            int flip = Random.Range(0, 2);
            return (flip == 0) ? ENodeState.Success : ENodeState.Failure;
        }

        //private ENodeState Log()
        //{

        //}
    }
}