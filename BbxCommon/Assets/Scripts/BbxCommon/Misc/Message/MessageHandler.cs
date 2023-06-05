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
    /// to avoid GC overhead, consider using <see cref="MessageQueueHandler{TMessageKey}"/> for listening.
    /// </para>
    /// </summary>
    public class MessageHandler<TMessageKey> : PooledObject, IMessageDispatcher<TMessageKey>, IMessageListener<TMessageKey>
    {
        #region Normal Callback
        private Dictionary<TMessageKey, UnityAction<MessageData>> m_Callbacks = new();

        public void AddCallback(TMessageKey messageKey, UnityAction<MessageData> callback)
        {
            m_Callbacks[messageKey] += callback;
        }

        public void RemoveCallback(TMessageKey messageKey, UnityAction<MessageData> callback)
        {
            m_Callbacks[messageKey] -= callback;
        }

        public void Dispatch(TMessageKey messageKey, MessageData messageData = null)
        {
            if (m_Callbacks.TryGetValue(messageKey, out var callback))
            {
                callback?.Invoke(messageData);
            }
            if (Listeners.TryGetValue(messageKey, out var set))
            {
                foreach (var listener in set)
                {
                    listener.OnRespond(messageKey, messageData);
                }
            }
        }

        void IMessageListener<TMessageKey>.OnRespond(TMessageKey messageKey, MessageData messageData)
        {
            m_Callbacks[messageKey].Invoke(messageData);
            if (Listeners.TryGetValue(messageKey, out var set))
            {
                foreach (var listener in set)
                {
                    listener.OnRespond(messageKey, messageData);
                }
            }
        }
        #endregion

        #region Listener
        internal Dictionary<TMessageKey, HashSet<IMessageListener<TMessageKey>>> Listeners = new();

        public void AddListener(TMessageKey messageKey, IMessageListener<TMessageKey> listener)
        {
            if (Listeners.TryGetValue(messageKey, out var set) == false)
            {
                set = SimplePool<HashSet<IMessageListener<TMessageKey>>>.Alloc();
                Listeners[messageKey] = set;
            }
            set.Add(listener);
        }

        public void RemoveListener(TMessageKey messageKey, IMessageListener<TMessageKey> listener)
        {
            Listeners[messageKey].Remove(listener);
        }
        #endregion

        #region Collect to Pool
        public override void OnCollect()
        {
            m_Callbacks.Clear();
            foreach (var pair in Listeners)
            {
                pair.Value.CollectToPool();
            }
            Listeners.CollectToPool();
        }
        #endregion
    }
}
