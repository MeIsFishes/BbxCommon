
namespace BbxCommon
{
    public enum ETaskRunState
    {
        Running,
        Succeeded,
        Failed,
    }

    public class TaskBase
    {
        private bool m_Running = false;

        public void Run()
        {
            TaskManager.Instance.RunningTasks.Add(this);
        }

        internal void Enter() { OnEnter(); }
        internal ETaskRunState Update() { return OnUpdate(); }
        internal void Exit() { OnExit(); }

        protected virtual void OnEnter() { }
        protected virtual ETaskRunState OnUpdate() { return ETaskRunState.Succeeded; }
        protected virtual void OnExit() { }
    }
}
