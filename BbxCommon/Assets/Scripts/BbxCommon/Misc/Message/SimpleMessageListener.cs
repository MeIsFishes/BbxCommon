using UnityEngine.Events;

namespace BbxCommon
{
    /// <summary>
    /// A listener for listening maybe just one message.
    /// </summary>
    public class SimpleMessageListener<TMessageKey> : PooledObject, IMessageListener<TMessageKey>
    {
        public UnityAction<MessageDataBase> Callback;

        void IMessageListener<TMessageKey>.OnRespond(TMessageKey messageKey, MessageDataBase messageData)
        {
            Callback?.Invoke(messageData);
        }

        public override void OnCollect()
        {
            Callback = null;
        }
    }
}
