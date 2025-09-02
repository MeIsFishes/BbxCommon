using System;
using System.Collections.Generic;
using System.Reflection;
using BbxCommon.Internal;
using LitJson;
using Unity.Entities.UniversalDelegates;

namespace BbxCommon
{
    internal class TaskManager : Singleton<TaskManager>
    {
        internal enum ERunningTaskState
        {
            NewEnter,
            Keep,
        }

        internal struct RunningTaskInfo
        {
            public TaskBase Task;
            public ERunningTaskState State;

            public RunningTaskInfo(TaskBase task, ERunningTaskState state)
            {
                Task = task;
                State = state;
            }
        }

        internal List<TaskBase> NewEnterTasks = new();
        internal List<RunningTaskInfo> RunningTasks = new();

        private Dictionary<string, TaskBridgeGroupInfo> m_Tasks = new();

        internal void RegisterTask(string key, TaskBridgeGroupInfo value)
        {
            m_Tasks[key] = value;
        }

        internal void RegisterTask(string key, TaskGroupInfo value)
        {
            var bridge = new TaskBridgeGroupInfo();
            bridge.FromTaskGroupInfo(value);
            m_Tasks[key] = bridge;
        }

        internal void RunTask(TaskBase task)
        {
            NewEnterTasks.Add(task);
        }

        internal void RunTask(string key, TaskContextBase context)
        {
            if (m_Tasks.TryGetValue(key, out var taskGroupInfo) == false)
            {
                var textAsset = ResourceApi.LoadTextAsset(key);
                if (textAsset != null)
                {
                    var jsonData = JsonMapper.ToObject(textAsset.text);
                    var readTaskGroupInfo = JsonApi.Deserialize<TaskGroupInfo>(jsonData);
                    var bridge = new TaskBridgeGroupInfo();
                    bridge.FromTaskGroupInfo(readTaskGroupInfo);
                    RegisterTask(key, bridge);
                    taskGroupInfo = bridge;
                }
                else
                {
                    DebugApi.LogError("No such task: " + key);
                    return;
                }
            }
            if (taskGroupInfo.BindingContextType != context.GetType())
            {
                DebugApi.LogError("Context doesn't match! You pass in " + context.GetType().FullName + ", but the task " + key +
                    " requires " + taskGroupInfo.BindingContextType.Name);
                return;
            }
            // generate tasks
            context.Init(taskGroupInfo);
            var taskList = SimplePool<List<TaskBase>>.Alloc();
            for (int i = 0; i < taskGroupInfo.TaskValueInfos.Count; i++)
            {
                // deserialize tasks
                var taskValueInfo = taskGroupInfo.TaskValueInfos[i];
                var task = TaskDeserialiser.GetTaskPool(taskValueInfo.TaskTypeId, taskValueInfo.TaskType).AllocObj() as TaskBase;
                for (int j = 0; j < taskValueInfo.FieldInfos.Count; j++)
                {
                    var fieldInfo = taskValueInfo.FieldInfos[j];
                    task.ReadFieldInfo(fieldInfo.FieldEnumValue, fieldInfo, context);
                    fieldInfo.Inited = true;
                }
                taskList.Add(task);
            }
            for (int i = 0; i < taskGroupInfo.TaskValueInfos.Count; i++)
            {
                taskList[i].InitConnectPoint(taskList);
            }
            // read task reference
            for (int i = 0; i < taskGroupInfo.TaskValueInfos.Count; i++)
            {
                var task = taskList[i];
                var taskInfo = taskGroupInfo.TaskValueInfos[i];
                // read timeline info
                if (taskInfo.IsTimeline == true && task is TaskTimeline taskTimeline)
                {
                    for (int j = 0; j < taskInfo.TimelineItemInfos.Count; j++)
                    {
                        var timelineItemInfo = taskInfo.TimelineItemInfos[j];
                        var timelineItemIndex = timelineItemInfo.Id;
                        if (timelineItemIndex < 0 || timelineItemIndex >= taskList.Count)
                        {
                            DebugApi.LogError("Timeline required child key not found! Id: " + timelineItemInfo.Id + ", task key: " + key);
                        }
                        taskTimeline.ReadTimelineItem(timelineItemInfo, taskList[timelineItemIndex]);
                    }
                }
                // read conditions
                if (taskInfo.HasCondition)
                {
                    for (int j = 0; j < taskInfo.EnterConditionReferences.Count; j++)
                    {
                        var taskRef = taskList[taskInfo.EnterConditionReferences[j]];
                        if (taskRef is TaskConditionBase condition)
                        {
                            task.AddEnterCondition(condition);
                        }
                        else
                        {
                            DebugApi.LogError("The task you require is not a TaskCondition! Id: " + taskInfo.EnterConditionReferences[j] + ", task key: " + key);
                        }
                    }
                    for (int j = 0; j < taskInfo.ConditionReferences.Count; j++)
                    {
                        var taskRef = taskList[taskInfo.ConditionReferences[j]];
                        if (taskRef is TaskConditionBase condition)
                        {
                            task.AddCondition(condition);
                        }
                        else
                        {
                            DebugApi.LogError("The task you require is not a TaskCondition! Id: " + taskInfo.ConditionReferences[j] + ", task key: " + key);
                        }
                    }
                    for (int j = 0; j < taskInfo.ExitConditionReferences.Count; j++)
                    {
                        var taskRef = taskList[taskInfo.ExitConditionReferences[j]];
                        if (taskRef is TaskConditionBase condition)
                        {
                            task.AddExitCondition(condition);
                        }
                        else
                        {
                            DebugApi.LogError("The task you require is not a TaskCondition! Id: " + taskInfo.ExitConditionReferences[j] + ", task key: " + key);
                        }
                    }
                }
            }
            // get root task and run
            if (taskGroupInfo.RootTaskId < 0 || taskGroupInfo.RootTaskId >= taskList.Count)
            {
                DebugApi.LogError("Cannot find root task! id: " + taskGroupInfo.RootTaskId + ", task key: " + key);
                taskList.CollectToPool();
                return;
            }
            var root = taskList[taskGroupInfo.RootTaskId];
            RunTask(root);
            taskList.CollectToPool();
        }
    }
}
