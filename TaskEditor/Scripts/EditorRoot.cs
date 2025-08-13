using System;
using System.IO;
using BbxCommon.Internal;
using Godot;

namespace BbxCommon
{
	public partial class EditorRoot : BbxControl
	{
		[Export]
		public TimelineRoot TimelineRoot;
		[Export]
		public TaskGraphManager GraphRoot;
		[Export]
		public Control EmptyRoot;
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
			EventBus.RegisterEvent(EEvent.CurSaveTargetChanged, OnCurSaveTargetChanged);
			SettingsPanelButton.Pressed += OnSettingsPanelButton;
			SaveButton.Pressed += OnSaveButton;
            SaveAsButton.Pressed += OnSaveAsButton;
            LoadButton.Pressed += OnLoadButton;

			OnCurSaveTargetChanged();

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
			if (EditorModel.CurSaveTarget == null)
				return;
            var list = EditorDataStore.GetTaskContextInfoList();
            EditorModel.CurSaveTarget.BindingContextType = list[(int)index].TaskContextTypeName;
        }

		private void OnCurSaveTargetChanged()
		{
			if (EditorModel.CurSaveTarget == null)
			{
				EmptyRoot.Visible = true;
				TimelineRoot.Visible = false;
				GraphRoot.Visible = false;
			}
			else if (EditorModel.CurSaveTarget.IsTimeline)
			{
                EmptyRoot.Visible = false;
                TimelineRoot.Visible = true;
                GraphRoot.Visible = false;
                BindContextOption.Select(EditorModel.CurSaveTarget.BindingContextType.TryRemoveStart("TaskContext"));
			}
			else if (EditorModel.CurSaveTarget.IsGraphNode)
			{
				EmptyRoot.Visible = false;
				TimelineRoot.Visible = false;
				GraphRoot.Visible = true;
			}
			else
			{
				DebugApi.LogError("EditorRoot OnCurSaveTargetChanged: Unknow SaveTarget Type!");
			}
		}

        private void OnSettingsPanelButton()
		{
			EditorModel.SettingsPanel.Open();
		}

		private void OnSaveButton()
		{
			if (File.Exists(EditorModel.CurSaveTarget.FilePath + ".editor.json"))
			{
				EditorModel.CurSaveTarget.Save();
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
				EditorModel.CurSaveTarget.FilePath = s;
                EditorModel.CurSaveTarget.Save(s);
				EditorSettings.Instance.LastSaveTargetPath = s;
				EventBus.DispatchEvent(EEvent.SaveTargetListChanged);
            }, FileDialog.FileModeEnum.SaveFile, EditorSettings.Instance.LastSaveTargetPath, "*.editor.json");
        }

		private void OnLoadButton()
		{
            EditorModel.OpenFileDialog((s) =>
            {
                var saveTarget = (EditorModel.SaveTargetBase)JsonApi.Deserialize(s);
				EditorModel.SaveTargetList.Insert(0, saveTarget);
				EditorModel.CurSaveTarget = saveTarget;
				EventBus.DispatchEvent(EEvent.SaveTargetListChanged);
				EditorSettings.Instance.LastSaveTargetPath = s;
            }, FileDialog.FileModeEnum.OpenFile, EditorSettings.Instance.LastSaveTargetPath, "*.editor.json");
        }
    }
}
