using BbxCommon.Internal;
using Godot;
using System.Collections.Generic;

namespace BbxCommon
{
	public abstract partial class TaskNode : Control
	{
        #region Common
        [Export]
		public BbxButton NodeButton;

		public abstract TaskEditData TaskEditData { get; }

        public sealed override void _Ready()
        {
			NodeButton.Pressed += OnNodeButtonClick;
			OnReady();
        }

		/// <summary>
		/// Bind the specific task type, and clear all cached field infos.
		/// </summary>
		public void BindTask(string taskType)
		{
			TaskEditData.TaskType = taskType;
            TaskEditData.Fields.Clear();
			var taskInfo = EditorDataStore.GetTaskInfo(taskType);
			for (int i = 0; i < taskInfo.FieldInfos.Count; i++)
			{
				var fieldInfo = taskInfo.FieldInfos[i];
				var editField = new TaskEditField();
				editField.FieldName = fieldInfo.FieldName;
				editField.TypeInfo = fieldInfo.TypeInfo;
				editField.Value = string.Empty;
                TaskEditData.Fields.Add(editField);
			}
			OnBind(taskType);
		}

		protected virtual void OnReady() { }
		protected virtual void OnBind(string taskType) { }
        #endregion

        #region Callbacks
        private void OnNodeButtonClick()
		{
			EditorModel.CurSelectTaskNode = this;
		}
        #endregion
    }
}
