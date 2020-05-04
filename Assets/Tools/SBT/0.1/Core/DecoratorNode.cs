using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public abstract class DecoratorNode : BaseNode
    {
        public delegate ENodeState DecoratorDelegate(ENodeState _input);
        protected DecoratorDelegate m_decoratorDelegate;

        protected BaseNode m_childNode;

        public override void Init()
        {
            
        }

        public override ENodeState Process()
        {
            // Get child result
            ENodeState childResult = m_childNode.Process();

            // Process with decorator delegate
            return m_decoratorDelegate(childResult);
        }
    }
}