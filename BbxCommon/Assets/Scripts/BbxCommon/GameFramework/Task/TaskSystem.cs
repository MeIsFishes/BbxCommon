using System.Collections.Generic;
using Unity.Entities;

namespace BbxCommon
{
    [DisableAutoCreation]
    internal partial class TaskSystem : EcsMixSystemBase
    {
        private struct TaskFinishInfo
        {
            public int Index;
            public bool Succeeded;
        }

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
                taskManager.RunningTasks.Add(task);
            }
            taskManager.NewEnterTasks.Clear();

            // update
            var finishInfos = SimplePool<List<TaskFinishInfo>>.Alloc();
            for (int i = 0; i < taskManager.RunningTasks.Count; i++)
            {
                var task = taskManager.RunningTasks[i];
                var taskState = task.Update(TimeApi.DeltaTime);
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
                var task = taskManager.RunningTasks[finishInfo.Index];
                if (finishInfo.Succeeded)
                    task.OnNodeSucceeded();
                else
                    task.OnNodeFailed();
                task.Exit();
            }
            for (int i = finishInfos.Count - 1; i >= 0; i--)
            {
                taskManager.RunningTasks.RemoveAt(finishInfos[i].Index);
            }

            // collect collections
            finishInfos.CollectToPool();
        }
    }
}
