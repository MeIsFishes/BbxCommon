
namespace BbxCommon.Ui
{
    public abstract class UiModelItemBase : PooledObject
    {
        internal abstract IMessageDispatcher<int> MessageDispatcher { get; }
        public virtual void SetDirty() { }
    }

    /// <summary>
    /// Models is a database for storing data for UI items. You can initialize and uninitialize data items by
    /// override <see cref="IPooledObject.OnAllocate"/> and <see cref="IPooledObject.OnCollect"/>.
    /// </summary>
    public abstract class UiModelBase : UiModelItemBase
    {
        private MessageHandler<int> m_MessageHandler = new();
        internal override IMessageDispatcher<int> MessageDispatcher => m_MessageHandler;

        /// <summary>
        /// The index in <see cref="UiModelManager"/>.
        /// </summary>
        internal int Index;
    }
}
