using Godot;
using System;
using System.Windows;

namespace BbxCommon
{
	public partial class SettingsPanel : BbxControl
	{
        #region Common
        [Export]
        public BbxButton BackButton;
        [Export]
        public TextEdit TaskInfoPathEdit;
        [Export]
        public BbxButton TaskInfoPathImportButton;
        [Export]
        public BbxButton TaskInfoPathPathButton;

        protected override void OnUiInit()
        {
            EditorModel.SettingsPanel = this;

            BackButton.Pressed += OnBackButtonClick;

            OnUiInitTaskInfoPath();
        }

        protected override void OnUiOpen()
        {
            TaskInfoPathEdit.Text = EditorSettings.Instance.ExportInfoPath;

            OnUiOpenTaskInfoPath();
        }

        public void Open()
        {
            Visible = true;
        }

        private void OnBackButtonClick()
        {
            Visible = false;
        }
        #endregion

        #region Task Info Path
        private void OnUiInitTaskInfoPath()
        {
            TaskInfoPathEdit.TextChanged += () =>
            {
                EditorSettings.Instance.ExportInfoPath = TaskInfoPathEdit.Text;
            };
            TaskInfoPathEdit.TextSet += () =>
            {
                EditorSettings.Instance.ExportInfoPath = TaskInfoPathEdit.Text;
            };
            TaskInfoPathImportButton.Pressed += OnTaskInfoPathImportButton;
            TaskInfoPathPathButton.Pressed += OnTaskInfoPathPathButton;
        }

        private void OnUiOpenTaskInfoPath()
        {
            TaskInfoPathEdit.Text = EditorSettings.Instance.ExportInfoPath;
        }

        private void OnTaskInfoPathImportButton()
        {
            EditorDataStore.DeserializeAllTaskInfo();
        }

        private void OnTaskInfoPathPathButton()
        {
            EditorModel.OpenFileDialog((s) =>
            {
                TaskInfoPathEdit.Text = s;
            },
            FileDialog.FileModeEnum.OpenDir);
        }
        #endregion
    }
}
