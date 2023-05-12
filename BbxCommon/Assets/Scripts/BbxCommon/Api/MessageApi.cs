using UnityEngine.Events;

namespace BbxCommon
{
    public static class MessageApi
    {
        /// <summary>
        /// A global broadcaster which simply dispatch int key messages.
        /// </summary>
        private static MessageHandler<int> m_GlobalMessageDispatcher = new();

        public static void RegisterGlobalListener(int messageKey, UnityAction<MessageData> callback)
        {
            m_GlobalMessageDispatcher.RegisterListener(messageKey, callback);
        }

        public static void UnregisterGlobalListener(int messageKey, UnityAction<MessageData> callback)
        {
            m_GlobalMessageDispatcher.UnregisterListener(messageKey, callback);
        }

        public static void BroadcastMessage(int messageKey, MessageData messageData = null)
        {
            m_GlobalMessageDispatcher.Dispatch(messageKey, messageData);
        }
    }
}
