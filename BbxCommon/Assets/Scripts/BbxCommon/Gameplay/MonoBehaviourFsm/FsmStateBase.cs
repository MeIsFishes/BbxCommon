
namespace BbxCommon.Fsm
{
    public abstract class FsmStateBase : PooledObject
    {
        protected FsmStateBase m_LastState;
        protected FsmStateBase m_NextState;

        public void Enter(FsmStateBase lastState)
        {
            m_LastState = lastState;
            OnEnter(lastState);
        }

        public int Execute()
        {
            OnExecute();
            return CheckTransition();
        }

        public void Exit(FsmStateBase nextState)
        {
            m_NextState = nextState;
            OnExit(nextState);
        }

        public virtual void Init() { }

        protected abstract void OnEnter(FsmStateBase lastState);
        protected abstract void OnExecute();
        protected abstract void OnExit(FsmStateBase nextState);
        /// <summary>
        /// Check and return a state key which indexes to FsmManagerBase.m_States.
        /// </summary>
        protected abstract int CheckTransition();
    }
}
