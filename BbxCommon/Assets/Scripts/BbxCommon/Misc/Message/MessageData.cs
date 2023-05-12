
namespace BbxCommon
{
    public abstract class MessageData : PooledObject
    {
        /// <summary>
        /// Broadcast the message to global registered listeners in <see cref="MessageApi"/>.
        /// </summary>
        public void QuickBroadcast(int messageKey)
        {
            MessageApi.BroadcastMessage(messageKey, this);
        }
    }
}
