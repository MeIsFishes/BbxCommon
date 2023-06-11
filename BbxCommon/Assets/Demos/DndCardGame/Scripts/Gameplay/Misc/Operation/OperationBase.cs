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
        public void Enter()
        {
            OnEnter();
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
        }
        protected virtual void OnExit() { }
    }
}
