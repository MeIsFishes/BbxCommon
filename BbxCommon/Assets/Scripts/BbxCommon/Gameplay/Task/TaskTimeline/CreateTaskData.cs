
namespace BbxCommon
{
    public delegate TaskBase DelegateCreateTask(TaskTimelineContextBase context);

    public struct CreateTaskData
    {
        public static CreateTaskData Invalid = new CreateTaskData(-1, null);

        public float CreatingTime { get; private set; }
        /// <summary>
        /// Create, initialize the only one task, and then return it.
        /// </summary>
        public DelegateCreateTask CreatingTaskFunc { get; private set; }

        public CreateTaskData(float CreatingTime, DelegateCreateTask CreatingTaskFunc)
        {
            this.CreatingTime = CreatingTime;
            this.CreatingTaskFunc = CreatingTaskFunc;
        }
    }
}
