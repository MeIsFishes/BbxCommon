using System.Collections.Generic;

namespace BbxCommon
{
    /// <summary>
    /// <para>
    /// In <see cref="MessageQueueHandler{TMessageKey}"/>, messages will be push into a queue when getting from the dispatcher, instead of
    /// responding and discarding the message data at once.
    /// </para><para>
    /// <see cref="MessageQueueHandler{TMessageKey}"/> may help you to build up a buffer, and additionally, avoid delegate closure passing.
    /// Notice that messages in queue will never be removed until you call <see cref="TryDequeue(out Message)"/>.
    /// </para>
    /// </summary>
    public class MessageQueueHandler<TMessageKey> : PooledObject, IMessageListener<TMessageKey>
    {
        public struct Message
        {
            public TMessageKey MessageKey;
            public MessageDataBase MessageData;
        }

        private Queue<Message> m_MessageQueue = new();

        void IMessageListener<TMessageKey>.OnRespond(TMessageKey messageKey, MessageDataBase messageData)
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

        public override void OnCollect()
        {
            m_MessageQueue.Clear();
        }
    }
}
