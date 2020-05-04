using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public class Inverter : DecoratorNode
    {
        public Inverter(BaseNode _child)
        {
            m_childNode = _child;
            m_decoratorDelegate = Invert;
        }

        private ENodeState Invert(ENodeState _input)
        {
            switch (_input)
            {
                case ENodeState.Failure: return ENodeState.Success;
                case ENodeState.Success: return ENodeState.Failure;
                case ENodeState.Running: return ENodeState.Running;
                default: return ENodeState.Success;
            }
        }
    }
}