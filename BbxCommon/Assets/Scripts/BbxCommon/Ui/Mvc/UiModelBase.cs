
namespace BbxCommon
{
    /// <summary>
    /// Models is a database for storing data for UI items. You can initialize and uninitialize data items by
    /// override <see cref="IPooledObject.OnAllocate"/> and <see cref="IPooledObject.OnCollect"/>.
    /// </summary>
    public abstract class UiModelBase : ListenableBase
    {
        
    }
}
