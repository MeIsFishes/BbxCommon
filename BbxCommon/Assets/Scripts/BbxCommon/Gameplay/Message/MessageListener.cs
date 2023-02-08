using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public class MessageListener
    {
        public delegate void ProcessMessage(Message message);
        public MessageTubes MessageTubes;
        /// <summary>
        /// The function that process messages. This function should be set by users, or it will be null.
        /// </summary>
        public ProcessMessage ProcessMessageFunc;

        // See MessageBroadcaster.
        private uint m_order;

        public void AddToBroadcaster()
        {
            MessageBroadcaster.Instance.AddListener(this);
        }

        public void Destroy()
        {
            MessageBroadcaster.Instance.RemoveListener(this);
        }

        // Messages should be processed at once, either in MessageBroadcaster.
        public void RecieveMessage(Message message)
        {
            if (ProcessMessageFunc == null)
            {
                Debug.LogWarning("MessageListener's ProcessMessageFunc has not been set!");
            }
            else
            {
                ProcessMessageFunc(message);
            }
        }
    }
}
