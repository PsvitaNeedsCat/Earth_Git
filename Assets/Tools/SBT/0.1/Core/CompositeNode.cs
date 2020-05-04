using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public abstract class CompositeNode : BaseNode
    {
        public delegate ENodeState CompositeDelegate(ENodeState _input);

        protected List<BaseNode> m_childNodes;
        protected int m_currentChildIndex;
        protected CompositeDelegate m_processChildDelegate;
        // Processes first child node

        // Upon receiving result, checks if it can continue

        // If it can, chooses next child node


        public override void Init()
        {
            
        }

        public override ENodeState Process()
        {
            // Process first child
            ENodeState childResult = m_childNodes[m_currentChildIndex].Process();

            // Get result, check if can continue
            ENodeState result = m_processChildDelegate(childResult);

            return result;
        }
    }
}