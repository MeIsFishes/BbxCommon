using UnityEngine;

namespace BbxCommon.Ui
{
    public interface IModelItemHost
    {
        void SetDirty();
    }

    public enum EUiModelItemEvent
    {
        Dirty,
        Destroy,
    }

    public abstract class UiModelItemBase : PooledObject, IModelItemHost
    {
        internal virtual IMessageDispatcher<EUiModelItemEvent> MessageDispatcher { get; }
        public virtual void SetDirty() { }
    }

    public class UiModelItem<T> : UiModelItemBase
    {
        private T m_Value;
        private IModelItemHost m_Host;

        /// <summary>
        /// There should be only <see cref="SimpleMessageListener"/> adding to the <see cref="m_MessageHandler"/>.
        /// </summary>
        private MessageHandler<EUiModelItemEvent> m_MessageHandler = new();

        internal override IMessageDispatcher<EUiModelItemEvent> MessageDispatcher => m_MessageHandler;

        public T Value => m_Value;

        public UiModelItem(IModelItemHost host, T value = default(T))
        {
            m_Host = host;
            m_Value = value;
        }

        public void SetValue(T value)
        {
            m_Value = value;
            SetDirty();
        }

        public override void SetDirty()
        {
            m_MessageHandler.Dispatch(0);
            m_Host.SetDirty();
        }
    }
}
