using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BbxCommon.Internal;

namespace BbxCommon
{
    public enum EConditionType
    {
        EnterCondition,
        Condition,
        ExitCondition,
    }

    /// <summary>
    /// All Task data deserialized from exported files will be stored here.
    /// </summary>
    public static class EditorDataStore
	{
		private static List<TaskExportInfo> m_TaskInfos = new();
		private static Dictionary<string, TaskExportInfo> m_TaskInfoDic = new();
		private static List<TaskContextExportInfo> m_TaskContextInfos = new();
		private static Dictionary<string, TaskContextExportInfo> m_TaskContextInfoDic = new();
		private static Dictionary<string, TaskEnumExportInfo> m_EnumInfoDic = new();    // if a Task field is enum type, it will be deserialized

        public static void DeserializeAllTaskInfo()
        {
			m_TaskInfos.Clear();
			m_TaskInfoDic.Clear();
            m_TaskContextInfos.Clear();
            m_TaskContextInfoDic.Clear();
            m_EnumInfoDic.Clear();
			DebugApi.Log("TaskDataStore: Cleared all task info. Starting deserialization...");

            var infoPath = EditorSettings.Instance.ExportInfoPath;
            if (Directory.Exists(infoPath))
            {
                foreach (var path in Directory.EnumerateFiles(infoPath))
                {
                    var obj = JsonApi.Deserialize(Path.GetFullPath(path));
                    if (obj is TaskExportInfo taskInfo)
                        AddTaskInfo(taskInfo);
                    else if (obj is TaskContextExportInfo contextInfo)
                        AddTaskContextInfo(contextInfo);
                    else if (obj is TaskEnumExportInfo enumInfo)
                        AddEnumInfo(enumInfo);
                }
				EventBus.DispatchEvent(EEvent.EditorDataStoreRefresh);
                DebugApi.Log("TaskDataStore: Deserialize task info finished!");
            }
			else
			{
				DebugApi.LogWarning("TaskDataStore: Task info path does not exist: " + infoPath);
			}
        }

        public static void AddTaskInfo(TaskExportInfo info)
		{
			m_TaskInfos.Add(info);
			m_TaskInfoDic.Add(info.TaskTypeName, info);
		}

		public static void AddTaskContextInfo(TaskContextExportInfo info)
		{
			m_TaskContextInfos.Add(info);
			m_TaskContextInfoDic.Add(info.TaskContextTypeName, info);
		}

		public static void AddEnumInfo(TaskEnumExportInfo info)
		{
			m_EnumInfoDic.TryAdd(info.EnumTypeName, info);
		}

		public static TaskExportInfo GetTaskInfo(string typeName)
		{
			if (m_TaskInfoDic.TryGetValue(typeName, out var taskInfo))
				return taskInfo;
			DebugApi.LogError("No such a TaskExportInfo: " + typeName);
			return null;
		}

		public static List<TaskExportInfo> GetTaskInfoList()
		{
			return m_TaskInfos;
		}

		public static TaskContextExportInfo GetTaskContextInfo(string typeName)
		{
			if (m_TaskContextInfoDic.TryGetValue(typeName, out var taskContextInfo) == false)
				DebugApi.LogError("No such a TaskContextExportInfo: " + typeName);
			return taskContextInfo;
		}

		public static List<TaskContextExportInfo> GetTaskContextInfoList()
		{
			return m_TaskContextInfos;
		}

		public static TaskEnumExportInfo GetEnumInfo(string enumName)
		{
			if (m_EnumInfoDic.TryGetValue(enumName, out var taskEnumInfo) == false)
				DebugApi.LogError("No such a TaskEnumExportInfo: " + enumName);
			return taskEnumInfo;
		}

        public static bool IsEnum(TaskExportTypeInfo typeInfo)
        {
			return m_EnumInfoDic.ContainsKey(typeInfo.TypeName);
        }

        public static bool IsEnum(string typeName)
        {
            return m_EnumInfoDic.ContainsKey(typeName);
        }
    }
}
