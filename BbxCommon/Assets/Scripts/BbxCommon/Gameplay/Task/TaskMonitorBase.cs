
namespace BbxCommon
{
    public abstract class TaskMonitorBase : TaskDurationBase
    {
        /// <summary>
        /// Call this function to initialize a monitor task instead of call InitTaskDuration() is recommended.
        /// </summary>
        public void InitTaskMonitor(float taskDuration)
        {
            InitTaskDuration(taskDuration, 0, true);
        }

        protected abstract bool IsActive();
        protected abstract void WhileActive();

        protected override void OnEnter() { }

        protected override void OnExit() { }

        protected override void OnInterval()
        {
            if (IsActive())
            {
                WhileActive();
            }
        }
    }
}
