using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BbxCommon
{
    internal static class TaskDeserialiser
    {
        #region Context Data
        public class ContextData
        {
            public bool Inited;
            public Dictionary<string, Type> FieldTypeDic = new();
            public Dictionary<string, int> FieldStrIndexDic = new();
        }

        private static List<ContextData> m_ContextDataList = new();

        public static ContextData GetContextData(int typeId)
        {
            if (m_ContextDataList.Count <= typeId)
            {
                m_ContextDataList.ModifyCount(typeId + 1);
                for (int i = 0; i < m_ContextDataList.Count; i++)
                {
                    if (m_ContextDataList[i] == null)
                        m_ContextDataList[i] = new();
                }
            }
            return m_ContextDataList[typeId];
        }

        public static ContextData GetContextData<TContext>() where TContext : TaskContextBase
        {
            var typeId = ClassTypeId<TaskContextBase, TContext>.Id;
            if (m_ContextDataList.Count <= typeId)
            {
                m_ContextDataList.ModifyCount(typeId + 1);
                for (int i = 0; i < m_ContextDataList.Count; i++)
                {
                    if (m_ContextDataList[i] == null)
                        m_ContextDataList[i] = new();
                }
            }
            return m_ContextDataList[typeId];
        }

        public static ContextData GetContextData(Type contextType)
        {
            var typeId = ClassTypeId<TaskContextBase>.GetId(contextType);
            if (m_ContextDataList.Count <= typeId)
            {
                m_ContextDataList.ModifyCount(typeId + 1);
                for (int i = 0; i < m_ContextDataList.Count; i++)
                {
                    if (m_ContextDataList[i] == null)
                        m_ContextDataList[i] = new();
                }
            }
            return m_ContextDataList[typeId];
        }

        public static ContextData GetContextData(TaskContextBase context)
        {
            var typeId = ClassTypeId<TaskContextBase>.GetId(context);
            if (m_ContextDataList.Count <= typeId)
            {
                m_ContextDataList.ModifyCount(typeId + 1);
                for (int i = 0; i < m_ContextDataList.Count; i++)
                {
                    if (m_ContextDataList[i] == null)
                        m_ContextDataList[i] = new();
                }
            }
            return m_ContextDataList[typeId];
        }
        #endregion

        #region Task Pool
        public static List<IObjectPoolHandler> TaskPools = new();

        public static IObjectPoolHandler GetTaskPool(int typeId, Type type)
        {
            if (TaskPools.Count <= typeId)
                TaskPools.ModifyCount(typeId + 1);
            if (TaskPools[typeId] == null)
                TaskPools[typeId] = ObjectPool.GetObjectPool(type);
            return TaskPools[typeId];
        }
        #endregion

        #region Task Data
        public class TaskData
        {
            public int CurEnumIndex;
            public Dictionary<string, int> FieldNameEnumDic = new();
        }

        private static List<TaskData> m_TaskDataList = new();

        public static int GetTaskFieldEnum(int typeId, string fieldName)
        {
            var taskData = GetTaskData(typeId);
            if (taskData.FieldNameEnumDic.TryGetValue(fieldName, out var enumValue) == false)
            {
                enumValue = taskData.CurEnumIndex++;
                taskData.FieldNameEnumDic.Add(fieldName, enumValue);
                return enumValue;
            }
            return enumValue;
        }

        private static TaskData GetTaskData(int typeId)
        {
            if (m_TaskDataList.Count <= typeId)
            {
                m_TaskDataList.ModifyCount(typeId + 1);
                for (int i = 0; i < m_TaskDataList.Count; i++)
                {
                    if (m_TaskDataList[i] == null)
                        m_TaskDataList[i] = new();
                }
            }
            return m_TaskDataList[typeId];
        }
        #endregion

        #region Task Type ID
        private static Dictionary<Type, int> m_TaskTypeIdDic = new();

        public static int GetTaskTypeId(Type type)
        {
            if (m_TaskTypeIdDic.TryGetValue(type, out var typeId) == false)
            {
                typeId = ClassTypeId<TaskBase>.GetId(type);
                m_TaskTypeIdDic[type] = typeId;
                return typeId;
            }
            return typeId;
        }
        #endregion
    }

    #region Bridge Structures
    [StructLayout(LayoutKind.Explicit)]
    public struct TaskBridgeConstValue
    {
        [FieldOffset(0)]
        public bool BoolValue;
        [FieldOffset(0)]
        public char CharValue;
        [FieldOffset(0)]
        public byte ByteValue;
        [FieldOffset(0)]
        public int IntValue;
        [FieldOffset(0)]
        public uint UintValue;
        [FieldOffset(0)]
        public long LongValue;
        [FieldOffset(0)]
        public ulong UlongValue;
        [FieldOffset(0)]
        public float FloatValue;
        [FieldOffset(0)]
        public double DoubleValue;
        [FieldOffset(0)]
        public decimal DecimalValue;
        [FieldOffset(0)]
        public short ShortValue;
        [FieldOffset(0)]
        public ushort UshortValue;
        [FieldOffset(8)]
        public object ObjectValue;
        [FieldOffset(24)]
        public string StringValue;
    }

    public class TaskBridgeFieldInfo
    {
        public bool Inited;
        public int FieldEnumValue;
        public ETaskFieldValueSource ValueSource;
        public TaskBridgeConstValue ConstValue;

        internal void FromTaskFieldInfo(TaskFieldInfo taskFieldInfo, int taskTypeId)
        {
            FieldEnumValue = TaskDeserialiser.GetTaskFieldEnum(taskTypeId, taskFieldInfo.FieldName);
            ValueSource = taskFieldInfo.ValueSource;
            ConstValue.StringValue = taskFieldInfo.Value;
        }
    }

    public class TaskBridgeValueInfo
    {
        public Type TaskType;
        public int TaskTypeId;
        public List<TaskBridgeFieldInfo> FieldInfos = new();  // Task fields
        public bool HasCondition;
        public List<int> EnterConditionReferences = new();
        public List<int> ConditionReferences = new();
        public List<int> ExitConditionReferences = new();
        public bool IsTimeline;
        public List<TaskTimelineItemInfo> TimelineItemInfos = new();    // TaskTimeline uses this struct

        internal void FromTaskValueInfo(TaskValueInfo taskValueInfo, Dictionary<int, int> reorderedIndexDic)
        {
            TaskType = ReflectionApi.GetType(taskValueInfo.FullTypeName);
            TaskTypeId = ClassTypeId<TaskBase>.GetId(TaskType);
            FieldInfos = taskValueInfo.FieldInfos.ConvertAll((fieldInfo) =>
            {
                var bridgeFieldInfo = new TaskBridgeFieldInfo();
                bridgeFieldInfo.FromTaskFieldInfo(fieldInfo, TaskTypeId);
                return bridgeFieldInfo;
            });
            EnterConditionReferences = new List<int>(taskValueInfo.EnterConditionReferences);
            for (int i = 0; i < EnterConditionReferences.Count; i++)
            {
                HasCondition = true;
                EnterConditionReferences[i] = reorderedIndexDic[EnterConditionReferences[i]];
            }
            ConditionReferences = new List<int>(taskValueInfo.ConditionReferences);
            for (int i = 0; i < ConditionReferences.Count; i++)
            {
                HasCondition = true;
                ConditionReferences[i] = reorderedIndexDic[ConditionReferences[i]];
            }
            ExitConditionReferences = new List<int>(taskValueInfo.ExitConditionReferences);
            for (int i = 0; i < ExitConditionReferences.Count; i++)
            {
                HasCondition = true;
                ExitConditionReferences[i] = reorderedIndexDic[ExitConditionReferences[i]];
            }
            for (int i = 0; i < taskValueInfo.TimelineItemInfos.Count; i++)
            {
                var info = new TaskTimelineItemInfo();
                info.StartTime = taskValueInfo.TimelineItemInfos[i].StartTime;
                info.Duration = taskValueInfo.TimelineItemInfos[i].Duration;
                info.Id = reorderedIndexDic[taskValueInfo.TimelineItemInfos[i].Id];
                TimelineItemInfos.Add(info);
            }
            // sort timeline items by start time
            if (TimelineItemInfos.Count > 0)
            {
                IsTimeline = true;
                TimelineItemInfos.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
            }
        }
    }

    public class TaskBridgeGroupInfo
    {
        public int RootTaskId;
        public Type BindingContextType;
        public List<TaskBridgeValueInfo> TaskValueInfos;
        public Dictionary<int, int> ReorderedIndexDic;

        public void FromTaskGroupInfo(TaskGroupInfo taskGroupInfo)
        {
            BindingContextType = ReflectionApi.GetType(taskGroupInfo.BindingContextFullType);
            // re-order tasks, let them be in continuous index and can be hit through list
            ReorderedIndexDic = new(taskGroupInfo.TaskInfos.Count);
            var tempCurIndex = 0;
            foreach (var pair in taskGroupInfo.TaskInfos)
            {
                if (ReorderedIndexDic.ContainsKey(pair.Key) == false)
                    ReorderedIndexDic.Add(pair.Key, tempCurIndex++);
            }
            TaskValueInfos = new List<TaskBridgeValueInfo>(tempCurIndex);
            TaskValueInfos.ModifyCount(tempCurIndex);
            foreach (var pair in taskGroupInfo.TaskInfos)
            {
                var bridgeValueInfo = new TaskBridgeValueInfo();
                bridgeValueInfo.FromTaskValueInfo(pair.Value, ReorderedIndexDic);
                var reorderedIndex = ReorderedIndexDic[pair.Key];
                TaskValueInfos[reorderedIndex] = bridgeValueInfo;
            }
            if (ReorderedIndexDic.ContainsKey(taskGroupInfo.RootTaskId) == false)
            {
                ReorderedIndexDic.CollectToPool();
            }
            RootTaskId = ReorderedIndexDic[taskGroupInfo.RootTaskId];
        }
    }
    #endregion
}
