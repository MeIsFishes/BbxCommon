using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public abstract class Message : PooledObject
    {
        public EMessageType MessageType { get; protected set; }

        public Message()
        {
            InitMessageType();
        }

        /// <summary>
        /// Broadcast the message without initializing.
        /// </summary>
        public static void QuickBroadcast<T>() where T : Message, new()
        {
            var message = ObjectPool<T>.Alloc();
            MessageBroadcaster.Instance.Broadcast(message);
            message.Collect();
        }

        // InitMessageType will be called in constructor.
        // Override this function in derived class.
        protected abstract void InitMessageType();
    }
}
