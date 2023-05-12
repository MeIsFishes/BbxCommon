using System.Collections.Generic;

namespace BbxCommon
{
    /// <summary>
    /// <para>
    /// In <see cref="MessageQueueHandler{TMessageKey}"/>, messages will be push into a queue when getting from the dispatcher, instead of
    /// responding and destroying the message data at once.
    /// </para><para>
    /// <see cref="MessageQueueHandler{TMessageKey}"/> may help you to build up a buffer, and additionally, avoid delegate closure passing.
    /// </para>
    /// </summary>
    public class MessageQueueHandler<TMessageKey> : PooledObject
    {
        public struct Message
        {
            public TMessageKey MessageKey;
            public MessageData MessageData;
        }

        internal MessageHandler<TMessageKey> Dispatcher;
        internal int Index;

        public void RegisterToDispatcher(TMessageKey messageKey, MessageHandler<TMessageKey> dispatcher)
        {
            if (dispatcher.MessageQueues.TryGetValue(messageKey, out var list) == false)
            {
                list = SimplePool<List<MessageQueueHandler<TMessageKey>>>.Alloc();
                dispatcher.MessageQueues[messageKey] = list;
            }
            Dispatcher = dispatcher;
            Index = list.Count;
            list.Add(this);
        }

        public void UnregisterMessageQueue()
        {

        }
    }
}
