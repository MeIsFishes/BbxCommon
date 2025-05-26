using System;
using System.IO;
using BbxCommon.Internal;
using Godot;

namespace BbxCommon
{
	public partial class EditorRoot : BbxControl
	{
		[Export]
		public OptionButton BindContextOption;

		private string m_ExportedInfoPath = "../ExportedTaskInfo/";

		protected override void OnUiOpen()
		{
			DeserializeAllTaskInfo();
			OnReadyBindContextOption();
			EditorModel.OnReady();
		}

		protected override void OnUiUpdate(double delta)
		{
			EditorModel.OnProcess(delta);
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
					else if (obj is TaskEnumExportInfo enumInfo)
						EditorDataStore.AddEnumInfo(enumInfo);
                }
				DebugApi.Log("Deserialize task info finished!");
			}
		}

		private void OnReadyBindContextOption()
		{
			BindContextOption.Clear();
			var contextList = EditorDataStore.GetTaskContextInfoList();
			for (int i = 0; i < contextList.Count; i++)
			{
				var contextInfo = contextList[i];
				BindContextOption.AddItem(contextInfo.TaskContextTypeName.TryRemoveStart("TaskContext"), i);
			}
			BindContextOption.ItemSelected += OnBindContextOptionSelect;
			// godot option menu cannot invoke selected callback when select via code
			OnBindContextOptionSelect(0);
        }

		private void OnBindContextOptionSelect(long index)
		{
            var list = EditorDataStore.GetTaskContextInfoList();
            EditorModel.BindingContextType = list[(int)index].TaskContextTypeName;
        }
    }
}
