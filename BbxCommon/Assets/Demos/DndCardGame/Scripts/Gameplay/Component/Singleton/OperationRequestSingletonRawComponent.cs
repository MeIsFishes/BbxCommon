using System.Collections.Generic;
using BbxCommon;

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
        public bool Blocked => m_Locks.Count > 0;
        public Queue<OperationBase> BlockedOperations = new();

        private HashSet<int> m_Locks = new();
        private UniqueIdGenerator m_LockIdGenerator = new();

        /// <summary>
        /// 使用这个函数添加请求而不要直接操作<see cref="BlockedOperations"/>，这是为未来联网做的预留
        /// </summary>
        public void AddBlockedOperation(OperationBase operation)
        {
            if (BlockedOperations.Count == 0)
                BlockedOperations.Enqueue(operation);
        }

        public int Block()
        {
            return (int)m_LockIdGenerator.GenerateID();
        }

        public void Unblock(int key)
        {
            m_Locks.Remove(key);
            if (m_Locks.Count == 0)
                m_LockIdGenerator.ResetCounter(0);
        }
        #endregion

        #region Free Operation
        // 自由操作。每帧都会被执行一个的操作。
        public Queue<OperationBase> FreeOperations = new();

        // 使用这个函数添加请求而不要直接操作Queue，这是为未来联网做的预留
        public void AddFreeOperation(OperationBase operation)
        {
            FreeOperations.Enqueue(operation);
        }
        #endregion

        #region Updating Operation
        public List<OperationBase> UpdatingOperations = new();
        #endregion

        #region Common
        public override void OnCollect()
        {
            BlockedOperations.CollectAndClearElements();
            FreeOperations.CollectAndClearElements();
            UpdatingOperations.CollectAndClearElements();
        }
        #endregion
    }
}
