using UnityEngine.Events;

namespace BbxCommon
{
    public struct ListenableItemListener
    {
        /// <summary>
        /// Stores <see cref="IListenable"/>. As the target may be collected during listening, and interfaces cannot be <see cref="ObjRef{T}"/>,
        /// we make a explicit converting.
        /// </summary>
        public ObjRef<PooledObject> ListenTarget;
        public int MessageKey;

        /// <summary>
        /// Adding a listener reference to <see cref="IMessageDispatcher{TMessageKey}"/> but not directly setting functions, to provide
        /// a GC-free delegate operation.
        /// </summary>
        private SimpleMessageListener<int> m_Listener;

        public ListenableItemListener(IListenable modelItem, int messageKey, UnityAction<MessageData> callback)
        {
            ListenTarget = ((PooledObject)modelItem).AsObjRef();
            MessageKey = messageKey;
            m_Listener = ObjectPool<SimpleMessageListener<int>>.Alloc();
            m_Listener.Callback += callback;
        }

        public void AddListener()
        {
            if (ListenTarget.IsNull())
                return;
            ((IListenable)ListenTarget.Obj).MessageDispatcher.AddListener(MessageKey, m_Listener);
        }

        public void TryRemoveListener()
        {
            if (ListenTarget.IsNotNull())
                ((IListenable)ListenTarget.Obj).MessageDispatcher.RemoveListener(MessageKey, m_Listener);
        }

        public void RebindTarget(IListenable item)
        {
            TryRemoveListener();
            ListenTarget = ((PooledObject)item).AsObjRef();
            AddListener();
        }

        /// <summary>
        /// In most cases, this function is unnecessary to call, unless the <see cref="UiControllerBase"/> need to be destroyed but not closed.
        /// </summary>
        public void ReleaseInfo()
        {
            m_Listener.CollectToPool();
        }
    }
}
