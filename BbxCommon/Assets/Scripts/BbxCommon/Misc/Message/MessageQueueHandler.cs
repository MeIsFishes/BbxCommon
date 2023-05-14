using System.Collections.Generic;

namespace BbxCommon
{
    /// <summary>
    /// <para>
    /// In <see cref="MessageQueueHandler{TMessageKey}"/>, messages will be push into a queue when getting from the dispatcher, instead of
    /// responding and discarding the message data at once.
    /// </para><para>
    /// <see cref="MessageQueueHandler{TMessageKey}"/> may help you to build up a buffer, and additionally, avoid delegate closure passing.
    /// Notice that messages in queue will never be removed until you call <see cref="TryDequeue(out MessageQueueHandler{TMessageKey}.Message)"/>.
    /// </para>
    /// </summary>
    public class MessageQueueHandler<TMessageKey> : PooledObject
    {
        #region Dispatch
        public struct Message
        {
            public TMessageKey MessageKey;
            public MessageData MessageData;
        }

        private Queue<Message> m_MessageQueue = new();

        internal void Dispatch(TMessageKey messageKey, MessageData messageData)
        {
            var message = new Message();
            message.MessageKey = messageKey;
            message.MessageData = messageData;
            m_MessageQueue.Enqueue(message);
        }

        public bool TryDequeue(out Message message)
        {
            return m_MessageQueue.TryDequeue(out message);
        }
        #endregion

        #region Register
        private struct RegisterInfo
        {
            public ObjRef<MessageHandler<TMessageKey>> Dispatcher;
            public TMessageKey MessageKey;
        }

        private List<RegisterInfo> m_RegisterInfos = new();

        public void RegisterToDispatcher(TMessageKey messageKey, MessageHandler<TMessageKey> dispatcher)
        {
            if (dispatcher.MessageQueues.TryGetValue(messageKey, out var set) == false)
            {
                set = SimplePool<HashSet<MessageQueueHandler<TMessageKey>>>.Alloc();
                dispatcher.MessageQueues[messageKey] = set;
            }
            set.Add(this);
            // registerInfo
            var info = new RegisterInfo();
            info.Dispatcher = dispatcher.AsObjRef();
            info.MessageKey = messageKey;
            m_RegisterInfos.Add(info);
        }

        public void UnregisterMessageQueue()
        {
            foreach (var info in m_RegisterInfos)
            {
                if (info.Dispatcher.IsNull() == false)
                {
                    var set = info.Dispatcher.Obj.MessageQueues[info.MessageKey];
                    set.Remove(this);
                }
            }
            m_RegisterInfos.Clear();
        }

        public override void OnCollect()
        {
            UnregisterMessageQueue();
            m_MessageQueue.Clear();
        }
        #endregion
    }
}
