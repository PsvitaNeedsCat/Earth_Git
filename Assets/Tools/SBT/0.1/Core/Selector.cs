using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public class Selector : CompositeNode
    {
        bool m_processedAllChildren = false;

        public override void Init()
        {
            base.Init();

            m_currentChildIndex = 0;
        }

        public Selector(List<BaseNode> _children)
        {
            m_processChildDelegate = ProcessChild;
            m_childNodes = _children;
        }

        ENodeState ProcessChild(ENodeState _childResult)
        {
            switch (_childResult)
            {
                case ENodeState.Failure:
                {
                    NextChild();
                    if (m_processedAllChildren) return ENodeState.Failure;
                    return ENodeState.Running;
                }
                case ENodeState.Success: return ENodeState.Success;
                case ENodeState.Running: return ENodeState.Running;
                case ENodeState.Fresh: return ENodeState.Fresh;
                default: return ENodeState.Failure;
            }
        }

        void NextChild()
        {
            m_currentChildIndex++;

            if (m_currentChildIndex >= m_childNodes.Count)
            {
                m_processedAllChildren = true;
            }
        }
    }
}