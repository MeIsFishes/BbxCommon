using BbxCommon.Internal;
using Godot;
using System;

namespace BbxCommon
{
	public partial class InspectorFieldItem : Node
	{
        #region Common
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
		private string m_FieldName;

        public override void _Ready()
        {
			OnReadyValueSourceOption();
			OnReadyCustomValueEdit();
			OnReadyPresetValueOption();
        }

        public void RebindField(TaskEditField editField)
        {
			ValueSourceOption.Visible = true;
			m_SpecialField = ESpecialField.None;
			m_FieldName = editField.FieldName;
			FieldNameLabel.Text = editField.FieldName;
			switch (editField.ValueSource)
			{
				case ETaskFieldValueSource.Value:
					ValueSourceOption.Select(0);
					OnValueSourceChanged(0);
					CustomValueEdit.Text = editField.Value;
					break;
				case ETaskFieldValueSource.Context:
					ValueSourceOption.Select(1);
					OnValueSourceChanged(1);
					var contextIndex = PresetValueOption.GetItemIndex(editField.Value);
					PresetValueOption.Select(contextIndex);
                    break;
				case ETaskFieldValueSource.Blackboard:
                    ValueSourceOption.Select(2);
					OnValueSourceChanged(2);
                    CustomValueEdit.Text = editField.Value;
                    break;
            }
        }

		public void RebindSpecialField(ESpecialField field)
		{
			switch (field)
			{
				case ESpecialField.TimelineStartTime:
					ValueSourceOption.Visible = false;
					m_SpecialField = ESpecialField.TimelineStartTime;
					FieldNameLabel.Text = "StartTime";
					if (EditorModel.CurSelectTaskNode.TaskEditData is TaskTimelineEditData timelineData1)
					{
						CustomValueEdit.Text = timelineData1.StartTime.ToString();
					}
					break;
				case ESpecialField.TimelineDuration:
                    ValueSourceOption.Visible = false;
                    m_SpecialField = ESpecialField.TimelineDuration;
                    FieldNameLabel.Text = "Duration";
                    if (EditorModel.CurSelectTaskNode.TaskEditData is TaskTimelineEditData timelineData2)
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
			// godot option menu cannot invoke selected callback when select via code
			OnValueSourceChanged(0);
        }

		private void OnReadyCustomValueEdit()
		{
			CustomValueEdit.TextChanged += (string content) => { ExportCurField(); };
		}

		private void OnReadyPresetValueOption()
		{
			PresetValueOption.ItemSelected += (long index) => { ExportCurField(); };
		}

		private void OnValueSourceChanged(long index)
		{
			switch (index)
			{
				case 0: // value
					CustomValueEdit.Visible = true;
					PresetValueOption.Visible = false;
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
			ExportCurField();
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

        #region Export Value
		private void ExportCurField()
		{
			if (EditorModel.CurSelectTaskNode == null)
				return;
			var editData = EditorModel.CurSelectTaskNode.TaskEditData;
			if (m_SpecialField == ESpecialField.TimelineStartTime)
			{
				if (editData is TaskTimelineEditData timelineData)
				{
					float.TryParse(CustomValueEdit.Text, out timelineData.StartTime);
				}
			}
			else if (m_SpecialField == ESpecialField.TimelineDuration)
			{
                if (editData is TaskTimelineEditData timelineData)
                {
                    float.TryParse(CustomValueEdit.Text, out timelineData.Duration);
                }
				var durationField = editData.GetEditField("Duration");
				if (durationField != null)
				{
					durationField.ValueSource = ETaskFieldValueSource.Value;
					durationField.Value = CustomValueEdit.Text;
				}
            }
			else if (m_SpecialField == ESpecialField.None)
			{
				var editField = editData.GetEditField(m_FieldName);
				if (editField == null)
					return;
				switch (ValueSourceOption.Selected)
				{
					case 0:
						editField.ValueSource = ETaskFieldValueSource.Value;
						editField.Value = CustomValueEdit.Text;
						break;
					case 1:
						editField.ValueSource = ETaskFieldValueSource.Context;
						editField.Value = PresetValueOption.GetItemText(PresetValueOption.Selected);
						break;
					case 2:
						editField.ValueSource = ETaskFieldValueSource.Blackboard;
						editField.Value = CustomValueEdit.Text;
						break;
				}
			}
		}
        #endregion
    }
}
