using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    /// <summary>
    /// 改变其他对象状态时都尽可能走Operation，这是为未来联网使用Lockstep（帧同步）做的预留
    /// </summary>
    public class OperationRequestSingletonRawComponent : EcsSingletonRawComponent
    {
        #region Blocked Operation
        // 可以被阻塞的操作。当被要求阻塞时，后续的操作不会被执行。

        /// <summary>
        /// 操作处理是否被要求阻塞
        /// </summary>
        public bool Blocked => m_LockItem.IsLocked;
        public Queue<BlockedOperationBase> BlockedOperations = new();

        private LockItem m_LockItem = new();
        private UniqueIdGenerator m_LockIdGenerator = new();

        /// <summary>
        /// 使用这个函数添加请求而不要直接操作<see cref="BlockedOperations"/>，这是为未来联网做的预留
        /// </summary>
        public void AddBlockedOperation(BlockedOperationBase operation)
        {
            if (BlockedOperations.Count == 0 && Blocked == false)
                BlockedOperations.Enqueue(operation);
            else
                UiApi.GetUiController<UiPromptController>()?.ShowPrompt("你现在不能下达指令！");
        }

        public LockItemKey Block()
        {
            return m_LockItem.Lock();
        }

        public void Unblock(LockItemKey key)
        {
            m_LockItem.Unlock(key);
        }

        #endregion

        #region Free Operation
        // 自由操作。每帧都会被执行一个的操作。
        public Queue<OrderedOperationBase> FreeOperations = new();

        // 使用这个函数添加请求而不要直接操作Queue，这是为未来联网做的预留
        public void AddFreeOperation(OrderedOperationBase operation)
        {
            FreeOperations.Enqueue(operation);
        }
        #endregion

        #region Updating Operation
        public List<OrderedOperationBase> UpdatingOperations = new();
        #endregion

        #region Common
        public override void OnAllocate()
        {
            m_LockItem = ObjectPool<LockItem>.Alloc();
        }

        public override void OnCollect()
        {
            m_LockItem.CollectToPool();
            BlockedOperations.CollectAndClearElements();
            FreeOperations.CollectAndClearElements();
            UpdatingOperations.CollectAndClearElements();
        }
        #endregion
    }
}
