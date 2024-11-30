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
	}
}
