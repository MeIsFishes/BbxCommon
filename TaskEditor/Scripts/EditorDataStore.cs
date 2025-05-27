using System.Collections.Generic;
using System.Reflection;
using BbxCommon.Internal;

namespace BbxCommon
{
	/// <summary>
	/// All Task data deserialized from exported files will be stored here.
	/// </summary>
	public static class EditorDataStore
	{
		private static List<TaskExportInfo> m_TaskInfos = new();
		private static Dictionary<string, TaskExportInfo> m_TaskInfoDic = new();
		private static List<TaskContextExportInfo> m_TaskContextInfos = new();
		private static Dictionary<string, TaskContextExportInfo> m_TaskContextInfoDic = new();
		private static Dictionary<string, TaskEnumExportInfo> m_EnumInfoDic = new();	// if a Task field is enum type, it will be deserialized

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
			return null;
		}

		public static List<TaskExportInfo> GetTaskInfoList()
		{
			return m_TaskInfos;
		}

		public static TaskContextExportInfo GetTaskContextInfo(string typeName)
		{
			m_TaskContextInfoDic.TryGetValue(typeName, out var taskContextInfo);
			return taskContextInfo;
		}

		public static List<TaskContextExportInfo> GetTaskContextInfoList()
		{
			return m_TaskContextInfos;
		}

		public static TaskEnumExportInfo GetEnumInfo(string enumName)
		{
			m_EnumInfoDic.TryGetValue(enumName, out var taskEnumInfo);
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
