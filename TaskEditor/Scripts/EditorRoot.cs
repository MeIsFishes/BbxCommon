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
		[Export]
		public BbxButton SettingsPanelButton;
		[Export]
		public BbxButton SaveButton;
		[Export]
		public BbxButton SaveAsButton;
		[Export]
		public BbxButton LoadButton;

		private string m_ExportedInfoPath = "../ExportedTaskInfo/";

        protected override void OnUiInit()
		{
			EventBus.RegisterEvent(EEvent.EditorDataStoreRefresh, OnReadyBindContextOption);
			SettingsPanelButton.Pressed += OnSettingsPanelButton;
			SaveButton.Pressed += OnSaveButton;
            SaveAsButton.Pressed += OnSaveAsButton;
            LoadButton.Pressed += OnLoadButton;

            EditorModel.EditorRoot = this;
            EditorModel.OnReady();
		}

		protected override void OnUiUpdate(double delta)
		{
			EditorModel.OnProcess(delta);
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

		private void OnSettingsPanelButton()
		{
			EditorModel.SettingsPanel.Open();
		}

		private void OnSaveButton()
		{
			if (File.Exists(EditorSettings.Instance.CurrentTaskPath))
			{
				EditorModel.SaveTarget.Save();
			}
			else
			{
				OnSaveAsButton();
			}
		}

		private void OnSaveAsButton()
		{
			EditorModel.OpenFileDialog((s) =>
			{
				EditorSettings.Instance.CurrentTaskPath = FileApi.AddExtensionIfNot(s, ".json");
                EditorModel.SaveTarget.Save();
            }, FileDialog.FileModeEnum.SaveFile, FileApi.GetDirectory(EditorSettings.Instance.CurrentTaskPath));
        }

		private void OnLoadButton()
		{
            EditorModel.OpenFileDialog((s) =>
            {
                EditorSettings.Instance.CurrentTaskPath = FileApi.AddExtensionIfNot(s, ".json");
                EditorModel.SaveTarget = (EditorModel.ISaveTarget)JsonApi.Deserialize(EditorSettings.Instance.CurrentTaskPath);
				EventBus.DispatchEvent(EEvent.ReloadEditingTaskData);
            }, FileDialog.FileModeEnum.OpenFile, FileApi.GetDirectory(EditorSettings.Instance.CurrentTaskPath));
        }
    }
}
