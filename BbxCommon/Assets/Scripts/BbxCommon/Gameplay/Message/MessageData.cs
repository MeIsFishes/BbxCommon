
namespace BbxCommon
{
    public abstract class MessageData : PooledObject
    {
        /// <summary>
        /// Broadcast the message to <see cref="GlobalMessageBroadcaster"/>.
        /// </summary>
        public void QuickBroadcast(int messageKey)
        {
            GlobalMessageBroadcaster.Instance.Broadcast(messageKey, this);
        }
    }
}
