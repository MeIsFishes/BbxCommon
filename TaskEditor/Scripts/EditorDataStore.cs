using System.Collections.Generic;
using BbxCommon.Internal;

namespace BbxCommon
{
	public static class EditorDataStore
	{
		private static List<TaskExportInfo> m_TaskInfos = new();
		private static Dictionary<string, TaskExportInfo> m_TaskInfoDic = new();
		private static List<TaskContextExportInfo> m_TaskContextInfos = new();
		private static Dictionary<string, TaskContextExportInfo> m_TaskContextInfoDic = new();

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
			if (m_TaskContextInfoDic.TryGetValue(typeName, out var taskContextInfo))
				return taskContextInfo;
			return null;
		}

		public static List<TaskContextExportInfo> GetTaskContextInfoList()
		{
			return m_TaskContextInfos;
		}
	}
}
