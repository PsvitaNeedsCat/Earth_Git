using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public abstract class LeafNode : BaseNode
    {
        public delegate ENodeState ActionDelegate();
        protected ActionDelegate m_actionDelegate;

        public override void Init()
        {

        }

        public override ENodeState Process()
        {
            ENodeState result = m_actionDelegate();

            m_nodeState = result;
            return m_nodeState;
        }
    }
}