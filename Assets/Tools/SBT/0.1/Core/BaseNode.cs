using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public enum ENodeState
    {
        Failure,
        Success,
        Running,
        Fresh // Has been reset, ready to be initialised
    }

    public abstract class BaseNode
    {
        protected ENodeState m_nodeState = ENodeState.Fresh;
        public ENodeState NodeState
        {
            get { return m_nodeState; }
        }

        // Called before the first time this node is processed, each time the tree runs
        public abstract void Init();

        public abstract ENodeState Process();
    }
}