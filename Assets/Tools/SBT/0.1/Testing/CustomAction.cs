using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public class CustomAction : LeafNode
    {
        public CustomAction(ActionDelegate _action)
        {
            m_actionDelegate = _action;
        }
    }
}