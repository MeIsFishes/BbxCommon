using System;
using System.Collections.Generic;
using System.Reflection;
using BbxCommon.Internal;

namespace BbxCommon
{
    internal class TaskManager : Singleton<TaskManager>
    {
        internal List<TaskBase> NewEnterTasks = new();
        internal List<TaskBase> RunningTasks = new();

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
                DebugApi.LogError("No such task: " + key);
                return;
            }
            if (taskGroupInfo.BindingContextFullType != context.GetType().FullName)
            {
                DebugApi.LogError("Context doesn't match! You pass in " + context.GetType().FullName + ", but the task " + key +
                    " requires " + taskGroupInfo.BindingContextFullType);
                return;
            }
            // generate tasks
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
                foreach (var refrenceInfo in taskInfo.TaskRefrenceDic)
                {
                    var enumType = task.GetFieldEnumType();
                    var enumValue = Enum.Parse(enumType, refrenceInfo.FieldName);
                    var taskList = SimplePool<List<TaskBase>>.Alloc();
                    for (int i = 0; i < refrenceInfo.Ids.Count; i++)
                    {
                        taskList.Add(taskDic[refrenceInfo.Ids[i]]);
                    }
                    task.ReadRefrenceInfo(enumValue.GetHashCode(), taskList, context);
                    taskList.CollectToPool();
                }
            }
            // get root task and run
            if (taskDic.TryGetValue(taskGroupInfo.RootTaskId, out var root) == false)
            {
                DebugApi.LogError("Cannot find root task! id: " + taskGroupInfo.RootTaskId + ", task key: " + key);
                return;
            }
            RunTask(root);
        }

        /// <summary>
        /// Deserialize only <see cref="TaskFieldInfo"/>, <see cref="TaskRefrenceInfo"/> will be deserialized in <see cref="RunTask(string, TaskContextBase)"/>.
        /// </summary>
        private TaskBase DeserializeTask(TaskValueInfo taskValueInfo, TaskContextBase context)
        {
            var type = ReflectionApi.GetType(taskValueInfo.FullTypeName, taskValueInfo.AssemblyQualifiedName);
            if (type == null)
            {
                DebugApi.LogError("Invalid Task Type, FullTypeName = " + taskValueInfo.FullTypeName + ", AssemblyQualifiedName = " + taskValueInfo.AssemblyQualifiedName);
                return null;
            }
            var task = Activator.CreateInstance(type) as TaskBase;
            for (int i = 0; i < taskValueInfo.FieldInfos.Count; i++)
            {
                var fieldInfo = taskValueInfo.FieldInfos[i];
                var enumType = task.GetFieldEnumType();
                var enumValue = Enum.Parse(enumType, fieldInfo.FieldName);
                task.ReadFieldInfo(enumValue.GetHashCode(), fieldInfo, context);
            }
            return task;
        }
    }
}
