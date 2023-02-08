using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    /// <summary>
    /// A singleton broadcaster. You can add listener to broadcaster and broadcast a
    /// message to all listeners that has been added.
    /// </summary>
    public class MessageBroadcaster : Singleton<MessageBroadcaster>
    {
        private HashSet<MessageListener> m_ListenerSet = new HashSet<MessageListener>();

        public void AddListener(MessageListener listener)
        {
            m_ListenerSet.Add(listener);
        }

        public void RemoveListener(MessageListener listener)
        {
            m_ListenerSet.Remove(listener);
        }

        /// <summary>
        /// Broadcast a message to all the listeners registered in boradcaster.
        /// </summary>
        public void Broadcast(Message message)
        {
            
            foreach (var listner in m_ListenerSet)
            {
                if (listner.MessageTubes.IsTypeMatch((uint)1 << (int)message.MessageType))
                {
                    if (listner.ProcessMessageFunc == null)
                    {
                        Debug.LogWarning("MessageListener's ProcessMessageFunc has not been set!");
                    }
                    else
                    {
                        listner.ProcessMessageFunc(message);
                    }
                }
            }
        }
    }
}
