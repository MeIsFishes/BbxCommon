
namespace BbxCommon
{
    /// <summary>
    /// Classes inherit this interface can be added to <see cref="MessageHandler{TMessageKey}"/> through
    /// <see cref="IMessageDispatcher{TMessageKey}.AddListener(TMessageKey, IMessageListener{TMessageKey})"/>.
    /// </summary>
    public interface IMessageListener<TMessageKey>
    {
        void OnRespond(TMessageKey messageKey, MessageDataBase messageData);
    }

    /// <summary>
    /// Classes inherit this interface can dispath message and invoke <see cref="IMessageListener{TMessageKey}.OnRespond(TMessageKey, MessageDataBase)"/>
    /// to process messages.
    /// </summary>
    public interface IMessageDispatcher<TMessageKey>
    {
        void Dispatch(TMessageKey messageKey, MessageDataBase messageData = null);
        void AddListener(TMessageKey messageKey, IMessageListener<TMessageKey> listener);
        void RemoveListener(TMessageKey messageKey, IMessageListener<TMessageKey> listener);
    }
}
