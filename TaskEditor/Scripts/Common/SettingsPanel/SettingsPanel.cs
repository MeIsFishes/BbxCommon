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
            TaskInfoPathEdit.Text = EditorSettings.Instance.TaskInfoPath;

            OnUiOpenTaskInfoPath();
        }

        public void Open()
        {
            Visible = true;
        }

        private void OnBackButtonClick()
        {
            EditorSettings.Instance.Save();
            Visible = false;
        }
        #endregion

        #region Task Info Path
        private void OnUiInitTaskInfoPath()
        {
            TaskInfoPathEdit.TextChanged += () =>
            {
                EditorSettings.Instance.TaskInfoPath = TaskInfoPathEdit.Text;
            };
            TaskInfoPathEdit.TextSet += () =>
            {
                EditorSettings.Instance.TaskInfoPath = TaskInfoPathEdit.Text;
            };
            TaskInfoPathImportButton.Pressed += OnTaskInfoPathImportButton;
            TaskInfoPathPathButton.Pressed += OnTaskInfoPathPathButton;
        }

        private void OnUiOpenTaskInfoPath()
        {
            TaskInfoPathEdit.Text = EditorSettings.Instance.TaskInfoPath;
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
