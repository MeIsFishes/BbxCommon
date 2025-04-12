using BbxCommon.Internal;
using Godot;
using System;

namespace BbxCommon
{
	public static class TaskUtils
	{
		public static string GetTaskDisplayName(string taskType)
		{
            taskType = taskType.TryRemoveStart("TaskNode");
            taskType = taskType.TryRemoveStart("Task");
			return taskType;
        }

		public static bool IsEnum(TaskExportTypeInfo typeInfo)
		{
			return EditorDataStore.IsEnum(typeInfo);
		}

		public static TaskEnumExportInfo GetEnumInfo(string typeName)
		{
			return EditorDataStore.GetEnumInfo(typeName);
		}

		public static TaskEnumExportInfo GetEnumInfo(TaskExportTypeInfo typeInfo)
		{
			return EditorDataStore.GetEnumInfo(typeInfo.TypeName);
		}
	}
}
