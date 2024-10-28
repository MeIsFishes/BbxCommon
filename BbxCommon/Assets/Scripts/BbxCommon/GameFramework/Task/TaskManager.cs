using System.Collections.Generic;

namespace BbxCommon
{
    internal class TaskManager : Singleton<TaskManager>
    {
        internal List<TaskBase> NewEnterTasks = new();
        internal List<TaskBase> RunningTasks = new();

        internal void RunTask(TaskBase task)
        {
            NewEnterTasks.Add(task);
        }
    }
}
