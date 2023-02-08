
namespace BbxCommon
{
    public abstract class TaskOnceBase : TaskBase
    {
        protected abstract void DoOnce();

        protected override void OnEnter()
        {
            DoOnce();
        }

        protected override ETaskState OnExecute()
        {
            return ETaskState.Finished;
        }

        protected override void OnExit() { }
    }
}
