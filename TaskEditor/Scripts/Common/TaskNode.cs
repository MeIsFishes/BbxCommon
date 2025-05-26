using BbxCommon.Internal;
using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
	public abstract partial class TaskNode : Control
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

        public sealed override void _Ready()
        {
			NodeButton.Pressed += OnNodeButtonClick;
			AddInspectorButton();
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
