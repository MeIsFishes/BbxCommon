
namespace BbxCommon.Ui
{
    public interface IUiModelItem
    {
        public IMessageDispatcher<int> MessageDispatcher { get; }
    }

    public abstract class UiModelItemBase : PooledObject, IUiModelItem
    {
        protected MessageHandler<int> m_MessageHandler = new();
        public IMessageDispatcher<int> MessageDispatcher => m_MessageHandler;
    }

    /// <summary>
    /// Models is a database for storing data for UI items. You can initialize and uninitialize data items by
    /// override <see cref="IPooledObject.OnAllocate"/> and <see cref="IPooledObject.OnCollect"/>.
    /// </summary>
    public abstract class UiModelBase : UiModelItemBase
    {
        
    }
}
