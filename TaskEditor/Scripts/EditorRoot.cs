using System;
using System.IO;
using BbxCommon.Internal;
using Godot;

namespace BbxCommon
{
	public partial class EditorRoot : Node
	{
		private string m_ExportedInfoPath = "../ExportedTaskInfo/";

		public override void _Ready()
		{
			DeserializeAllTaskInfo();
		}

		public override void _Process(double delta)
		{

		}

		private void DeserializeAllTaskInfo()
		{
			if (Directory.Exists(m_ExportedInfoPath))
			{
				foreach (var path in Directory.EnumerateFiles(m_ExportedInfoPath))
				{
					var obj = JsonApi.Deserialize(Path.GetFullPath(path));
					if (obj is TaskExportInfo taskInfo)
						EditorDataStore.AddTaskInfo(taskInfo);
					else if (obj is TaskContextExportInfo contextInfo)
						EditorDataStore.AddTaskContextInfo(contextInfo);
                }
				DebugApi.Log("Finished!");
			}
		}
	}
}
