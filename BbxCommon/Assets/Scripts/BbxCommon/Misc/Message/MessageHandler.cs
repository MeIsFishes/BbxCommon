using System.Collections.Generic;
using UnityEngine.Events;

namespace BbxCommon
{
    /// <summary>
    /// <para>
    /// A handler to dispatch and listen messages.
    /// </para><para>
    /// Responding messages with delegate is the easist way to process message data, and it shows a full calling
    /// stack of how message sends and be processed.
    /// </para><para>
    /// However, as delegates are often with closure data, GC is always a problem. In that case, if you need
    /// to avoid GC overhead, consider using <see cref="MessageQueueHandler{TMessageKey}"/> to listen.
    /// </para>
    /// </summary>
    public class MessageHandler<TMessageKey> : PooledObject
    {
        private Dictionary<TMessageKey, UnityAction<MessageData>> m_Callbacks = new();
        internal Dictionary<TMessageKey, List<MessageQueueHandler<TMessageKey>>> MessageQueues = new();

        public void RegisterListener(TMessageKey messageKey, UnityAction<MessageData> callback)
        {
            m_Callbacks[messageKey] += callback;
        }

        public void UnregisterListener(TMessageKey messageKey, UnityAction<MessageData> callback)
        {
            m_Callbacks[messageKey] -= callback;
        }

        public void Dispatch(TMessageKey messageKey, MessageData messageData = null)
        {
            if (m_Callbacks.TryGetValue(messageKey, out var callback))
            {
                callback?.Invoke(messageData);
            }
        }

        public override void OnCollect()
        {
            m_Callbacks.Clear();
        }
    }
}
