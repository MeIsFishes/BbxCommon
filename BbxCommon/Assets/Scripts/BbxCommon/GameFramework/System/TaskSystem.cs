using System.Collections.Generic;
using Unity.Entities;

namespace BbxCommon
{
    [DisableAutoCreation]
    internal partial class TaskSystem : EcsMixSystemBase
    {
        // A new-entered task will execute OnEnter(), and then execute OnUpdate(deltaTime) once with deltaTime = 0, and turn to normal next frame.
        protected override void OnSystemUpdate()
        {
            var taskManager = TaskManager.Instance;
            if (taskManager.NewEnterTasks.Count == 0 && taskManager.RunningTasks.Count == 0)
                return;

            // new enter
            for (int i = 0; i < taskManager.NewEnterTasks.Count; i++)
            {
                var task = taskManager.NewEnterTasks[i];
                task.Enter();
                taskManager.RunningTasks.Add(new TaskManager.RunningTaskInfo(task, TaskManager.ERunningTaskState.NewEnter));
            }
            taskManager.NewEnterTasks.Clear();

            // update
            var finishedTaskIndex = SimplePool<List<int>>.Alloc();
            for (int i = 0; i < taskManager.RunningTasks.Count; i++)
            {
                var taskInfo = taskManager.RunningTasks[i];
                var taskState = ETaskRunState.Running;
                switch (taskInfo.State)
                {
                    case TaskManager.ERunningTaskState.NewEnter:
                        taskState = taskInfo.Task.Update(0);
                        taskManager.RunningTasks[i] = new TaskManager.RunningTaskInfo(taskInfo.Task, TaskManager.ERunningTaskState.Keep);
                        break;
                    case TaskManager.ERunningTaskState.Keep:
                        taskState = taskInfo.Task.Update(TimeApi.DeltaTime);
                        break;
                }
                if (taskState == ETaskRunState.Succeeded || taskState == ETaskRunState.Failed)
                    finishedTaskIndex.Add(i);
            }

            // exit
            // Since in some extreme cases, running order may cause bugs, we promise that tasks always run in the order of adding.
            for (int i = 0; i < finishedTaskIndex.Count; i++)
            {
                var taskInfo = taskManager.RunningTasks[finishedTaskIndex[i]];
                taskInfo.Task.Exit();
            }
            for (int i = finishedTaskIndex.Count - 1; i >= 0; i--)
            {
                taskManager.RunningTasks[finishedTaskIndex[i]].Task.CollectToPool();
                taskManager.RunningTasks.RemoveAt(finishedTaskIndex[i]);
            }

            // collect collections
            finishedTaskIndex.CollectToPool();
        }
    }
}
