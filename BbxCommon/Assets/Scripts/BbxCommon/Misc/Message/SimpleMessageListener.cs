using UnityEngine.Events;

namespace BbxCommon
{
    /// <summary>
    /// A listener for listening just one event.
    /// </summary>
    public class SimpleMessageListener<TMessageKey> : PooledObject, IMessageListener<TMessageKey>
    {
        public UnityAction<MessageData> Callback;

        void IMessageListener<TMessageKey>.OnRespond(TMessageKey messageKey, MessageData messageData)
        {
            Callback?.Invoke(messageData);
        }

        public override void OnCollect()
        {
            Callback = null;
        }
    }
}
