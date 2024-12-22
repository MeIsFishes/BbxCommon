using BbxCommon.Internal;
using Godot;
using System.Collections.Generic;

namespace BbxCommon
{
	public partial class TaskEditField : GodotObject
	{
		public string FieldName;
        public TaskExportTypeInfo TypeInfo;
		public ETaskFieldValueSource ValueSource;
        public string Value;
	}

	public partial class TaskNode : Control
	{
        #region Common
        [Export]
		public BbxButton NodeButton;

		public string TaskType;
		public List<TaskEditField> Fields
		{
			get
			{
				var res = new List<TaskEditField>();
				res.AddList(m_Fields);
				return res;
			}
		}
		private List<TaskEditField> m_Fields = new();
		private Dictionary<string, TaskEditField> m_FieldDic = new();

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
			TaskType = taskType;
			m_Fields.Clear();
            m_FieldDic.Clear();
			var taskInfo = EditorDataStore.GetTaskInfo(taskType);
			for (int i = 0; i < taskInfo.FieldInfos.Count; i++)
			{
				var fieldInfo = taskInfo.FieldInfos[i];
				var editField = new TaskEditField();
				editField.FieldName = fieldInfo.FieldName;
				editField.TypeInfo = fieldInfo.TypeInfo;
				editField.Value = string.Empty;
				AddEditField(editField);
			}
			OnBind(taskType);
		}

		protected virtual void OnReady() { }
		protected virtual void OnBind(string taskType) { }
        #endregion

        #region Interfaces
		public void AddEditField(TaskEditField field)
		{
			m_Fields.Add(field);
			m_FieldDic.Add(field.FieldName, field);
		}

        public void InsertEditField(int index, TaskEditField field)
        {
			if (m_FieldDic.ContainsKey(field.FieldName))
			{
				for (int i = 0; i < m_Fields.Count; i++)
				{
					if (m_Fields[i].FieldName == field.FieldName)
					{
						m_Fields.RemoveAt(i);
						break;
					}
				}
				m_FieldDic.Remove(field.FieldName);
			}
            m_Fields.Insert(index, field);
            m_FieldDic.Add(field.FieldName, field);
        }

        public TaskEditField GetEditField(string fieldName)
		{
			m_FieldDic.TryGetValue(fieldName, out var editField);
			return editField;
		}
        #endregion

        #region Callbacks
        private void OnNodeButtonClick()
		{
			EditorRuntime.CurSelectTaskNode = this;
		}
        #endregion
    }
}
