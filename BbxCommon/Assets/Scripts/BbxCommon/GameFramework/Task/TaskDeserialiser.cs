using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
using static BbxCommon.TaskBase;

namespace BbxCommon
{
    internal static class TaskDeserialiser
    {
        #region Context
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
                m_ContextDataList.ModifyCount(typeId + 1);
            if (m_ContextDataList[typeId] == null)
                m_ContextDataList[typeId] = new();
            return m_ContextDataList[typeId];
        }

        public static ContextData GetContextData<TContext>() where TContext : TaskContextBase
        {
            var typeId = ClassTypeId<TaskContextBase, TContext>.Id;
            if (m_ContextDataList.Count <= typeId)
                m_ContextDataList.ModifyCount(typeId + 1);
            if (m_ContextDataList[typeId] == null)
                m_ContextDataList[typeId] = new();
            return m_ContextDataList[typeId];
        }

        public static ContextData GetContextData(Type contextType)
        {
            var typeId = ClassTypeId<TaskContextBase>.GetId(contextType);
            if (m_ContextDataList.Count <= typeId)
                m_ContextDataList.ModifyCount(typeId + 1);
            if (m_ContextDataList[typeId] == null)
                m_ContextDataList[typeId] = new();
            return m_ContextDataList[typeId];
        }

        public static ContextData GetContextData(TaskContextBase context)
        {
            var typeId = ClassTypeId<TaskContextBase>.GetId(context);
            if (m_ContextDataList.Count <= typeId)
                m_ContextDataList.ModifyCount(typeId + 1);
            if (m_ContextDataList[typeId] == null)
                m_ContextDataList[typeId] = new();
            return m_ContextDataList[typeId];
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
        [FieldOffset(16)]
        public object ObjectValue;
        [FieldOffset(32)]
        public string StringValue;
    }

    public class TaskBridgeFieldInfo
    {
        public bool Inited;
        public string FieldName;
        public int FieldEnumValue;
        public ETaskFieldValueSource ValueSource;
        public TaskBridgeConstValue ConstValue;

        internal void FromTaskFieldInfo(TaskFieldInfo taskFieldInfo)
        {
            FieldName = taskFieldInfo.FieldName;
            ValueSource = taskFieldInfo.ValueSource;
            ConstValue.StringValue = taskFieldInfo.Value;
        }
    }

    public class TaskBridgeValueInfo
    {
        public Type TaskType;
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
            FieldInfos = taskValueInfo.FieldInfos.ConvertAll((fieldInfo) =>
            {
                var bridgeFieldInfo = new TaskBridgeFieldInfo();
                bridgeFieldInfo.FromTaskFieldInfo(fieldInfo);
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
        public List<TaskBridgeValueInfo> TaskInfos;

        public void FromTaskGroupInfo(TaskGroupInfo taskGroupInfo)
        {
            BindingContextType = ReflectionApi.GetType(taskGroupInfo.BindingContextFullType);
            // re-order tasks, let them be in continuous index and can be hit through list
            var tempIndexDic = SimplePool<Dictionary<int, int>>.Alloc();
            var tempCurIndex = 0;
            foreach (var pair in taskGroupInfo.TaskInfos)
            {
                if (tempIndexDic.ContainsKey(pair.Key) == false)
                    tempIndexDic.Add(pair.Key, tempCurIndex++);
            }
            TaskInfos = new List<TaskBridgeValueInfo>(tempCurIndex);
            TaskInfos.ModifyCount(tempCurIndex);
            foreach (var pair in taskGroupInfo.TaskInfos)
            {
                var bridgeValueInfo = new TaskBridgeValueInfo();
                bridgeValueInfo.FromTaskValueInfo(pair.Value, tempIndexDic);
                var reorderedIndex = tempIndexDic[pair.Key];
                TaskInfos[reorderedIndex] = bridgeValueInfo;
            }
            if (tempIndexDic.ContainsKey(taskGroupInfo.RootTaskId) == false)
            {
                tempIndexDic.CollectToPool();
            }
            RootTaskId = tempIndexDic[taskGroupInfo.RootTaskId];
            tempIndexDic.CollectToPool();
        }
    }
    #endregion
}
