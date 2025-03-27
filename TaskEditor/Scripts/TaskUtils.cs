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
	}
}
