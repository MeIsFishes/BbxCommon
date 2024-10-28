using System.Collections.Generic;
using Unity.Entities;

namespace BbxCommon
{
    [DisableAutoCreation]
    internal partial class TaskSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
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
            taskManager.RunningTasks.Clear();

            // update
            var succeededTasks = SimplePool<List<TaskBase>>.Alloc();
            var finishedTaskIndexes = SimplePool<List<int>>.Alloc();
            for (int i = 0; i < taskManager.RunningTasks.Count; i++)
            {
                var task = taskManager.RunningTasks[i];
                var taskState = task.Update();
                switch (taskState)
                {
                    case ETaskRunState.Succeeded:
                        succeededTasks.Add(task);
                        finishedTaskIndexes.Add(i);
                        break;
                    case ETaskRunState.Failed:
                        finishedTaskIndexes.Add(i);
                        break;
                }
            }

            // exit
            for (int i = 0; i < succeededTasks.Count; i++)
            {
                succeededTasks[i].Exit();
            }
            for (int i = finishedTaskIndexes.Count - 1; i >= 0; i--)
            {
                // We don't use the method taskManager.RunningTasks.UnorderedRemoveAt(), to ensure tasks always run in the order of adding,
                // since in some extreme cases, running order may cause bugs.
                taskManager.RunningTasks.RemoveAt(finishedTaskIndexes[i]);
            }

            // collect collections
            succeededTasks.CollectToPool();
            finishedTaskIndexes.CollectToPool();
        }
    }
}
