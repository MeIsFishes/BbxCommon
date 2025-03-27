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

		public TaskEditData TaskEditData;

        public sealed override void _Ready()
        {
			NodeButton.Pressed += OnNodeButtonClick;
			OnReady();
        }

		/// <summary>
		/// Bind the specific task type, and clear all cached field infos.
		/// </summary>
		public void BindTask(TaskEditData editData)
		{
			TaskEditData = editData;
			OnBind(editData);
		}

		protected virtual void OnReady() { }
		protected virtual void OnBind(TaskEditData editData) { }
        #endregion

        #region Callbacks
        private void OnNodeButtonClick()
		{
			EditorModel.CurSelectTaskNode = this;
		}
        #endregion
    }
}
