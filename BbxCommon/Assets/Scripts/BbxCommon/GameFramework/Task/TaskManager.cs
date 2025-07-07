using System;
using System.Collections.Generic;
using System.Reflection;
using BbxCommon.Internal;
using LitJson;
using UnityEditor.Graphs;

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

        private Dictionary<string, TaskGroupInfo> m_Tasks = new();

        internal void RegisterTask(string key, TaskGroupInfo value)
        {
            m_Tasks[key] = value;
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
                    RegisterTask(key, readTaskGroupInfo);
                    taskGroupInfo = readTaskGroupInfo;
                }
                else
                {
                    DebugApi.LogError("No such task: " + key);
                    return;
                }
            }
            if (taskGroupInfo.BindingContextFullType != context.GetType().FullName &&
                taskGroupInfo.BindingContextFullType != context.GetType().Name)
            {
                DebugApi.LogError("Context doesn't match! You pass in " + context.GetType().FullName + ", but the task " + key +
                    " requires " + taskGroupInfo.BindingContextFullType);
                return;
            }
            // generate tasks
            context.Init();
            var taskDic = SimplePool<Dictionary<int, TaskBase>>.Alloc();
            foreach (var pair in taskGroupInfo.TaskInfos)
            {
                taskDic[pair.Key] = DeserializeTask(pair.Value, context);
            }
            // read task reference
            foreach (var pair in taskGroupInfo.TaskInfos)
            {
                var task = taskDic[pair.Key];
                var taskInfo = pair.Value;
                foreach (var refrenceInfo in taskInfo.TaskRefrences)
                {
                    var enumTypes = new List<Type>();
                    task.GetFieldEnumTypes(enumTypes);
                    foreach (var enumType in enumTypes)
                    {
                        var enumValue = Enum.Parse(enumType, refrenceInfo.FieldName);
                        var taskList = SimplePool<List<TaskBase>>.Alloc();
                        for (int i = 0; i < refrenceInfo.Ids.Count; i++)
                        {
                            taskList.Add(taskDic[refrenceInfo.Ids[i]]);
                        }
                        task.ReadRefrenceInfo(enumValue.GetHashCode(), taskList, context);
                        taskList.CollectToPool();
                    }
                    enumTypes.CollectToPool();
                }
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
        /// Deserialize only <see cref="TaskFieldInfo"/>. <see cref="TaskRefrenceInfo"/> will be deserialized in <see cref="RunTask(string, TaskContextBase)"/>.
        /// </summary>
        private TaskBase DeserializeTask(TaskValueInfo taskValueInfo, TaskContextBase context)
        {
            var type = ReflectionApi.GetType(taskValueInfo.FullTypeName);
            if (type == null)
            {
                DebugApi.LogError("Invalid Task Type, FullTypeName = " + taskValueInfo.FullTypeName);
                return null;
            }
            var task = Activator.CreateInstance(type) as TaskBase;
            for (int i = 0; i < taskValueInfo.FieldInfos.Count; i++)
            {
                var fieldInfo = taskValueInfo.FieldInfos[i];
                var enumTypes = new List<Type>();
                task.GetFieldEnumTypes(enumTypes);
                foreach (var enumType in enumTypes)
                {
                    var ok = Enum.TryParse(enumType, fieldInfo.FieldName, out var enumValue);
                    if (ok)
                    {
                        task.ReadFieldInfo(enumValue.GetHashCode(), fieldInfo, context);
                        break;
                    }
                }
            }
            return task;
        }
    }
}
