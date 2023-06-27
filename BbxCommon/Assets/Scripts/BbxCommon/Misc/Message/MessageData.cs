
namespace BbxCommon
{
    public abstract class MessageDataBase : PooledObject
    {
        /// <summary>
        /// Broadcast the message to global registered listeners in <see cref="MessageApi"/>.
        /// </summary>
        public void QuickBroadcast(int messageKey)
        {
            MessageApi.GlobalMessageDispatcher.Dispatch(messageKey, this);
        }
    }
}
