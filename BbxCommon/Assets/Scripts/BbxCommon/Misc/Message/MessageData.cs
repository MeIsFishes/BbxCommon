
namespace BbxCommon
{
    public class MessageData : PooledObject
    {
        /// <summary>
        /// A quick way for storing a single data object.
        /// </summary>
        internal object Data;

        /// <summary>
        /// If you ensure that there is a data object in the <see cref="MessageData"/>, it is a quick way to get it out.
        /// </summary>
        public T GetData<T>()
        {
            return (T)Data;
        }

        /// <summary>
        /// Broadcast the message to global registered listeners in <see cref="MessageApi"/>.
        /// </summary>
        public void QuickBroadcast(int messageKey)
        {
            MessageApi.GlobalMessageDispatcher.Dispatch(messageKey, this);
        }
    }
}
