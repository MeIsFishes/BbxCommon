
namespace BbxCommon.Ui
{
    public abstract class UiModelItemBase : PooledObject
    {
        internal abstract IMessageDispatcher<int> MessageDispatcher { get; }
        public virtual void SetDirty() { }
    }

    public abstract class UiModelBase : UiModelItemBase
    {
        private MessageHandler<int> m_MessageHandler = new();
        internal override IMessageDispatcher<int> MessageDispatcher => m_MessageHandler;
    }
}
