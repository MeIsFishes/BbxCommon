using UnityEngine.Events;

namespace BbxCommon
{
    /// <summary>
    /// A global broadcaster which simply dispatch int key messages.
    /// </summary>
    public class GlobalMessageBroadcaster : Singleton<GlobalMessageBroadcaster>
    {
        private MessageDispatcher<int> m_MessageDispatcher;

        public void RegisterListener(int messageKey, UnityAction<MessageData> callback)
        {
            m_MessageDispatcher.RegisterListener(messageKey, callback);
        }

        public void UnregisterListener(int messageKey, UnityAction<MessageData> callback)
        {
            m_MessageDispatcher.UnregisterListener(messageKey, callback);
        }

        public void Broadcast(int messageKey, MessageData messageData = null)
        {
            m_MessageDispatcher.Dispatch(messageKey, messageData);
        }

        protected override void OnSingletonInit()
        {
            base.OnSingletonInit();
            m_MessageDispatcher = ObjectPool.AllocIfNull(m_MessageDispatcher);
        }

        protected override void OnSingletonDestroy()
        {
            base.OnSingletonDestroy();
            m_MessageDispatcher.CollectToPool();
        }
    }
}
