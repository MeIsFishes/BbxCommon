using BbxCommon.Internal;
using Godot;
using System;

namespace BbxCommon
{
	/// <summary>
	/// There are two field types: normal field and special field.
	/// 
	/// Normal field represents the fields in Task that developer claimed.
	/// 
	/// Special field is those ones claimed in base classes, for example, start time and duration in TaskTimeline.
	/// And maybe there will be some fields only used in editor.
	/// However, special field means it should be processed differently from normal ones.
	/// </summary>
	public partial class InspectorFieldItem : Node
	{
        #region Common

        #region Bind Field
        [Export]
		public Label FieldNameLabel;
		[Export]
		public OptionButton ValueSourceOption;
		[Export]
		public LineEdit CustomValueEdit;
		[Export]
		public OptionButton PresetValueOption;

		public enum ESpecialField
		{
			None,
			TimelineStartTime,
			TimelineDuration,
		}

		private ESpecialField m_SpecialField;
		private TaskEditField m_EditFieldInfo;

        public override void _Ready()
        {
			OnReadyValueSourceOption();
        }

		/// <summary>
		/// Bind normal fields.
		/// </summary>
        public void RebindField(TaskEditField editField)
        {
			ValueSourceOption.Visible = true;
			m_SpecialField = ESpecialField.None;
			m_EditFieldInfo = editField;
			FieldNameLabel.Text = editField.FieldName;
			switch (editField.ValueSource)
			{
				case ETaskFieldValueSource.Value:
                    ValueSourceOption.Select(0);
                    OnValueSourceChanged(0);
					if (TaskUtils.IsEnum(editField.TypeInfo))
					{
						PresetValueOption.Select(editField.Value);
					}
					else
					{
						CustomValueEdit.Text = editField.Value;
					}
                    break;
				case ETaskFieldValueSource.Context:
                    ValueSourceOption.Select(1);
                    OnValueSourceChanged(1);
                    PresetValueOption.Select(editField.Value);
                    break;
				case ETaskFieldValueSource.Blackboard:
                    ValueSourceOption.Select(2);
                    OnValueSourceChanged(2);
                    CustomValueEdit.Text = editField.Value;
                    break;
            }
        }

		/// <summary>
		/// If there are more special fields in future, write in this function.
		/// </summary>
		public void RebindSpecialField(ESpecialField field, TaskNode taskNode)
		{
			m_EditFieldInfo = null;
			switch (field)
			{
				case ESpecialField.TimelineStartTime:
					ValueSourceOption.Visible = false;
					m_SpecialField = ESpecialField.TimelineStartTime;
					FieldNameLabel.Text = "StartTime";
					if (taskNode.TaskEditData is TaskTimelineEditData timelineData1)
					{
						CustomValueEdit.Text = timelineData1.StartTime.ToString();
					}
					break;
				case ESpecialField.TimelineDuration:
                    ValueSourceOption.Visible = false;
                    m_SpecialField = ESpecialField.TimelineDuration;
                    FieldNameLabel.Text = "Duration";
                    if (taskNode.TaskEditData is TaskTimelineEditData timelineData2)
                    {
                        CustomValueEdit.Text = timelineData2.Duration.ToString();
                    }
                    break;
			}
		}

		private void OnReadyValueSourceOption()
		{
            ValueSourceOption.AddItem("Value", 0);
            ValueSourceOption.AddItem("Context", 1);
            ValueSourceOption.AddItem("Blackboard", 2);
			ValueSourceOption.ItemSelected += OnValueSourceChanged;
        }

		private void OnValueSourceChanged(long index)
		{
			switch (index)
			{
				case 0: // value
					if (TaskUtils.IsEnum(m_EditFieldInfo.TypeInfo))
					{
                        CustomValueEdit.Visible = false;
                        PresetValueOption.Visible = true;
						var enumInfo = TaskUtils.GetEnumInfo(m_EditFieldInfo.TypeInfo);
						RefreshEnumFields(enumInfo);
                    }
					else
					{
						CustomValueEdit.Visible = true;
						PresetValueOption.Visible = false;
					}
					break;
				case 1: // context
					CustomValueEdit.Visible = false;
					PresetValueOption.Visible = true;
					RefreshContextFields();
					break;
				case 2: // blackboard
					CustomValueEdit.Visible = true;
					PresetValueOption.Visible = false;
					break;
			}
		}
        #endregion

        #region Preset Values
		private void RefreshEnumFields(TaskEnumExportInfo enumInfo)
		{
			PresetValueOption.Clear();
			for (int i = 0; i < enumInfo.EnumValues.Count; i++)
			{
				PresetValueOption.AddItem(enumInfo.EnumValues[i]);
			}
		}

        private void RefreshContextFields()
		{
			PresetValueOption.Clear();
			var contextInfo = EditorModel.BindingContextInfo;
			for (int i = 0; i < contextInfo.FieldInfos.Count; i++)
			{
				PresetValueOption.AddItem(contextInfo.FieldInfos[i].FieldName);
			}
		}
        #endregion

        #endregion

        #region Export Value
        public void ExportCurField(TaskNode node)
		{
			if (EditorModel.CurSelectTaskNode == null)
				return;
			var editData = node.TaskEditData;
			if (m_SpecialField == ESpecialField.TimelineStartTime)
			{
				if (editData is TaskTimelineEditData timelineData)
				{
					float.TryParse(CustomValueEdit.Text, out var startTime);
					timelineData.StartTime = startTime;
                }
				EventBus.DispatchEvent(EEvent.TimelineNodeStartTimeOrDurationChanged);
            }
			else if (m_SpecialField == ESpecialField.TimelineDuration)
			{
                if (editData is TaskTimelineEditData timelineData)
                {
                    float.TryParse(CustomValueEdit.Text, out var duration);
                    timelineData.Duration = duration;
                }
				var durationField = editData.GetEditField("Duration");
				if (durationField != null)
				{
					durationField.ValueSource = ETaskFieldValueSource.Value;
					durationField.Value = CustomValueEdit.Text;
				}
                EventBus.DispatchEvent(EEvent.TimelineNodeStartTimeOrDurationChanged);
            }
			else if (m_SpecialField == ESpecialField.None)
			{
				if (m_EditFieldInfo == null)
					return;
				switch (ValueSourceOption.Selected)
				{
					case 0:	// value
                        m_EditFieldInfo.ValueSource = ETaskFieldValueSource.Value;
						if (TaskUtils.IsEnum(m_EditFieldInfo.TypeInfo))
						{
							m_EditFieldInfo.Value = PresetValueOption.GetItemText(PresetValueOption.Selected);
						}
						else
						{
							m_EditFieldInfo.Value = CustomValueEdit.Text;
						}
						break;
					case 1:	// context
                        m_EditFieldInfo.ValueSource = ETaskFieldValueSource.Context;
                        m_EditFieldInfo.Value = PresetValueOption.GetItemText(PresetValueOption.Selected);
						break;
					case 2:	// blackboard
                        m_EditFieldInfo.ValueSource = ETaskFieldValueSource.Blackboard;
                        m_EditFieldInfo.Value = CustomValueEdit.Text;
						break;
				}
			}
		}
        #endregion
    }
}
