using UnityEngine.Events;

namespace BbxCommon
{
    /// <summary>
    /// Classes inherit this interface can be added to <see cref="MessageHandler{TMessageKey}"/> through
    /// <see cref="MessageHandler{TMessageKey}.AddListener(TMessageKey, IMessageListener{TMessageKey})"/>.
    /// </summary>
    public interface IMessageListener<TMessageKey>
    {
        void OnRespond(TMessageKey messageKey, MessageData messageData);
    }

    /// <summary>
    /// A listener for listening maybe just one message.
    /// </summary>
    public class SimpleMessageListener : IMessageListener<int>
    {
        public UnityAction Callback;

        void IMessageListener<int>.OnRespond(int messageKey, MessageData messageData)
        {
            Callback?.Invoke();
        }
    }
}
