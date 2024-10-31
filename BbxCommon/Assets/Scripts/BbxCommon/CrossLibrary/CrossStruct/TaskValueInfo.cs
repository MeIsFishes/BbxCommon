using System.Collections.Generic;

namespace BbxCommon.Internal
{
    public enum ETaskFieldValueSource
    {
        Value,
        Context,
        Blackboard,
    }

    public struct TaskFieldInfo
    {
        public string FiledName;
        public ETaskFieldValueSource ValueSource;
        public string Value;
    }

    public struct TaskRefrenceInfo
    {
        public int Id;
    }

    public struct TaskTimelineItemInfo
    {
        public float StartTime;
        public float Duration;
        public int Id;
    }

    public class TaskValueInfo
    {
        public string FullTypeName;
        public string AssemblyQualifiedName;
        public List<TaskFieldInfo> FieldInfos = new();
        public Dictionary<string, TaskRefrenceInfo> TaskRefrenceDic = new();
        public List<TaskTimelineItemInfo> TimelineItemInfos = new();
    }

    public class TaskGroupInfo
    {
        public string BindingContextFullType;
        public Dictionary<int, TaskValueInfo> TaskInfos = new();
    }
}
