using BbxCommon.Internal;
using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
	public abstract partial class TaskNode : BbxControl
	{
        #region Common
		public struct ButtonData
		{
			public string Name;
			public Action Callback;
		}

        [Export]
		public BbxButton NodeButton;

		public TaskEditData TaskEditData;
		public List<ButtonData> InspectorButtonDatas = new();

        protected sealed override void OnUiOpen()
        {
			NodeButton.Pressed += OnNodeButtonClick;
			AddInspectorButton();
			OnTaskUiOpen();
        }

        protected sealed override void OnUiClose()
        {
			OnTaskUiClose();
            NodeButton.Pressed -= OnNodeButtonClick;
        }

        /// <summary>
        /// Bind the specific task type, and clear all cached field infos.
        /// </summary>
        public void BindTask(TaskEditData editData)
		{
			TaskEditData = editData;
			OnBind(editData);
		}

		protected virtual void OnTaskUiOpen() { }
		protected virtual void OnTaskUiClose() { }
		protected virtual void AddInspectorButton() { }
		protected virtual void OnBind(TaskEditData editData) { }

		protected void AddButton(string name, Action callback)
		{
			InspectorButtonDatas.Add(new ButtonData { Name = name, Callback = callback });
		}
        #endregion

        #region Callbacks
        private void OnNodeButtonClick()
		{
			EditorModel.CurSelectTaskNode = this;
		}
        #endregion
    }
}
