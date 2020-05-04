using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBT
{
    public class LogNode : LeafNode
    {
        float m_lifetime;
        string m_startupLog;
        string m_finishLog;
        float m_timer = 0.0f;
        bool m_logged = false;

        public LogNode(float _lifetime, string _startupLog, string _finishLog)
        {
            m_lifetime = _lifetime;
            m_startupLog = _startupLog;
            m_finishLog = _finishLog;
            m_timer = m_lifetime;

            m_actionDelegate = LogDelegate;
        }

        private ENodeState LogDelegate()
        {
            if (!m_logged)
            {
                Debug.Log(m_startupLog);
                m_logged = true;
            }

            m_timer -= Time.deltaTime;

            if (m_timer > 0.0f)
            {
                return ENodeState.Running;
            }

            Debug.Log(m_finishLog);
            return ENodeState.Success;
        }
    }
}