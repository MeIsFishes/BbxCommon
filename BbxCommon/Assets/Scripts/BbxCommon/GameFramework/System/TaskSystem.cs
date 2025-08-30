using System.Collections.Generic;
using Unity.Entities;

namespace BbxCommon
{
    [DisableAutoCreation]
    internal partial class TaskSystem : EcsMixSystemBase
    {
        internal struct TaskFinishInfo
        {
            public int Index;
            public bool Succeeded;
        }

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
            var finishInfos = SimplePool<List<TaskFinishInfo>>.Alloc();
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
                var finishInfo = new TaskFinishInfo();
                switch (taskState)
                {
                    case ETaskRunState.Succeeded:
                        finishInfo.Index = i;
                        finishInfo.Succeeded = true;
                        finishInfos.Add(finishInfo);
                        break;
                    case ETaskRunState.Failed:
                        finishInfo.Index = i;
                        finishInfo.Succeeded = false;
                        finishInfos.Add(finishInfo);
                        break;
                }
            }

            // exit
            // Since in some extreme cases, running order may cause bugs, we promise that tasks always run in the order of adding.
            for (int i = 0; i < finishInfos.Count; i++)
            {
                var finishInfo = finishInfos[i];
                var taskInfo = taskManager.RunningTasks[finishInfo.Index];
                if (finishInfo.Succeeded)
                    taskInfo.Task.OnNodeSucceeded();
                else
                    taskInfo.Task.OnNodeFailed();
                taskInfo.Task.Exit();
            }
            for (int i = finishInfos.Count - 1; i >= 0; i--)
            {
                taskManager.RunningTasks[finishInfos[i].Index].Task.CollectToPool();
                taskManager.RunningTasks.RemoveAt(finishInfos[i].Index);
            }

            // collect collections
            finishInfos.CollectToPool();
        }
    }
}
