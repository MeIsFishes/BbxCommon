using System.Collections.Generic;
using UnityEngine.Events;

namespace BbxCommon
{
    /// <summary>
    /// Objects can hold a MessageDispatcher with a certain message key type, and others can listen messages they are interested in.
    /// </summary>
    public class MessageDispatcher<TMessageKey> : PooledObject
    {
        private Dictionary<TMessageKey, UnityAction<MessageData>> m_MessageCallbacks = new Dictionary<TMessageKey, UnityAction<MessageData>>();

        public void RegisterListener(TMessageKey messageKey, UnityAction<MessageData> callback)
        {
            if (m_MessageCallbacks.ContainsKey(messageKey))
                m_MessageCallbacks[messageKey] += callback;
            else
                m_MessageCallbacks[messageKey] = callback;
        }

        public void UnregisterListener(TMessageKey messageKey, UnityAction<MessageData> callback)
        {
            m_MessageCallbacks[messageKey] -= callback;
        }

        public void Dispatch(TMessageKey messageKey, MessageData messageData = null)
        {
            if (m_MessageCallbacks.TryGetValue(messageKey, out var callback))
                callback(messageData);
        }
    }
}
