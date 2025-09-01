using System;
using System.Collections.Generic;
using System.Reflection;
using BbxCommon.Internal;
using LitJson;

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
            context.Init();
            var taskDic = SimplePool<Dictionary<int, TaskBase>>.Alloc();
            foreach (var pair in taskGroupInfo.TaskInfos)
            {
                taskDic[pair.Key] = DeserializeTask(pair.Value, context);
            }
            foreach (var pair in taskDic)
            {
                pair.Value.InitConnectPoint(taskDic);
            }
            // read task reference
            foreach (var pair in taskGroupInfo.TaskInfos)
            {
                var task = taskDic[pair.Key];
                var taskInfo = pair.Value;
                // read timeline info
                if (task is TaskTimeline taskTimeline)
                {
                    for (int i = 0; i < taskInfo.TimelineItemInfos.Count; i++)
                    {
                        var timelineItemInfo = taskInfo.TimelineItemInfos[i];
                        if (taskDic.TryGetValue(timelineItemInfo.Id, out var childTask) == false)
                        {
                            DebugApi.LogError("Timeline required child key not found! Id: " + timelineItemInfo.Id + ", task key: " + key);
                        }
                        taskTimeline.ReadTimelineItem(timelineItemInfo, childTask);
                    }
                    taskTimeline.SortItems();
                }
                // read conditions
                for (int i = 0; i < taskInfo.EnterConditionReferences.Count; i++)
                {
                    var taskRef = taskDic[taskInfo.EnterConditionReferences[i]];
                    if (taskRef is TaskConditionBase condition)
                    {
                        task.AddEnterCondition(condition);
                    }
                    else
                    {
                        DebugApi.LogError("The task you require is not a TaskCondition! Id: " + taskInfo.EnterConditionReferences[i] + ", task key: " + key);
                    }
                }
                for (int i = 0; i < taskInfo.ConditionReferences.Count; i++)
                {
                    var taskRef = taskDic[taskInfo.ConditionReferences[i]];
                    if (taskRef is TaskConditionBase condition)
                    {
                        task.AddCondition(condition);
                    }
                    else
                    {
                        DebugApi.LogError("The task you require is not a TaskCondition! Id: " + taskInfo.ConditionReferences[i] + ", task key: " + key);
                    }
                }
                for (int i = 0; i < taskInfo.ExitConditionReferences.Count; i++)
                {
                    var taskRef = taskDic[taskInfo.ExitConditionReferences[i]];
                    if (taskRef is TaskConditionBase condition)
                    {
                        task.AddExitCondition(condition);
                    }
                    else
                    {
                        DebugApi.LogError("The task you require is not a TaskCondition! Id: " + taskInfo.ExitConditionReferences[i] + ", task key: " + key);
                    }
                }
            }
            // get root task and run
            if (taskDic.TryGetValue(taskGroupInfo.RootTaskId, out var root) == false)
            {
                DebugApi.LogError("Cannot find root task! id: " + taskGroupInfo.RootTaskId + ", task key: " + key);
                taskDic.CollectToPool();
                return;
            }
            RunTask(root);
            taskDic.CollectToPool();
        }

        /// <summary>
        /// Deserialize only <see cref="TaskFieldInfo"/>.
        /// </summary>
        private TaskBase DeserializeTask(TaskBridgeValueInfo taskValueInfo, TaskContextBase context)
        {
            var task = ObjectPool.Alloc(taskValueInfo.TaskType) as TaskBase;
            for (int i = 0; i < taskValueInfo.FieldInfos.Count; i++)
            {
                var fieldInfo = taskValueInfo.FieldInfos[i];
                if (fieldInfo.Inited == false)
                {
                    var enumTypes = new List<Type>();
                    task.GetFieldEnumTypes(enumTypes);
                    foreach (var enumType in enumTypes)
                    {
                        var ok = Enum.TryParse(enumType, fieldInfo.FieldName, out var enumValue);
                        if (ok) fieldInfo.FieldEnumValue = (int)enumValue;
                    }
                    task.ReadFieldInfo(fieldInfo.FieldEnumValue, fieldInfo, context);
                    fieldInfo.Inited = true;
                }
                else
                {
                    task.ReadFieldInfo(fieldInfo.FieldEnumValue, fieldInfo, context);
                }
            }
            return task;
        }
    }
}
