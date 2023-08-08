using BbxCommon;

namespace Dcg
{
    public enum EOperationState
    {
        Running,
        Finished,
    }

    public abstract class OperationBase : PooledObject
    {
        private LockItemKey m_BlockKey;

        public void Enter()
        {
            OnEnter();
            if (IsBlocked())
            {
                m_BlockKey = EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>().Block();
            }
        }
        protected virtual void OnEnter() { }

        public EOperationState Update(float deltaTime)
        {
            return OnUpdate(deltaTime);
        }
        protected virtual EOperationState OnUpdate(float deltaTime) { return EOperationState.Finished; }

        public void Exit()
        {
            OnExit();
            if (IsBlocked())
            {
                EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>().Unblock(m_BlockKey);
            }
        }
        protected virtual void OnExit() { }

        protected virtual bool IsBlocked() { return false; }
    }

    public abstract class BlockedOperationBase : OperationBase
    {
        protected sealed override bool IsBlocked()
        {
            return true;
        }
    }
}
