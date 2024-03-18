using BbxCommon.TaskInternal;

namespace BbxCommon
{
    public enum ETaskState
    {
        Running,
        Finished,
    }

    // You can consider task a series of logic are somehow managed by a ticker. You just need to
    // write what a task need to do, when to finish, and then it will run by a manager every frame
    // util it should finish.

    /// <summary>
    /// A series of logic managed by a ticker.
    /// </summary>
    public abstract class TaskBase : PooledObject
    {
        public bool HasExited;
        /// <summary>
        /// Store the TaskTimeline which the task belongs to. It will be set when created by a TaskTimeline.
        /// </summary>
        protected ObjRef<TaskTimeline> m_TaskTimelineBelongs;

        public void SetTaskTimelineBelongs(TaskTimeline taskTimeline)
        {
            m_TaskTimelineBelongs = new ObjRef<TaskTimeline>(taskTimeline);
        }

        public void Enter()
        {
            OnEnter();
        }

        public ETaskState Execute()
        {
            return OnExecute();
        }

        public void Exit()
        {
            HasExited = true;
            OnExit();
        }

        protected abstract void OnEnter();

        protected abstract ETaskState OnExecute();

        protected abstract void OnExit();

        /// <summary>
        /// Stop running and exit the task.
        /// </summary>
        public void Stop()
        {
            Exit();
        }

        public override void OnCollect()
        {
            HasExited = false;
            m_TaskTimelineBelongs.ReleaseRef();
        }
    }
}
