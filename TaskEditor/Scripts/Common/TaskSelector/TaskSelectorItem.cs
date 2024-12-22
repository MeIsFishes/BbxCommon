using BbxCommon.Internal;
using Godot;
using System;

namespace BbxCommon
{
	public partial class TaskSelectorItem : Control
	{
		[Export]
		public BbxButton Button;

		public TaskExportInfo TaskInfo { get; private set; }

		public void SetCallback(Action callback)
		{
			Button.Pressed += callback;
		}

		public void SetTaskInfo(TaskExportInfo taskInfo)
		{
			TaskInfo = taskInfo;
			Button.Text = taskInfo.TaskTypeName;
		}
	}
}
