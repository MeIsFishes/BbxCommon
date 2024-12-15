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

		private string m_FieldName;

        public override void _Ready()
        {
			OnReadyValueSourceOption();
			OnReadyCustomValueEdit();
			OnReadyPresetValueOption();
        }

        public void RebindField(TaskEditField editField)
        {
			m_FieldName = editField.FieldName;
			FieldNameLabel.Text = editField.FieldName;
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
			var contextInfo = EditorRuntime.BindingContextInfo;
			for (int i = 0; i < contextInfo.FieldInfos.Count; i++)
			{
				PresetValueOption.AddItem(contextInfo.FieldInfos[i].FieldName);
			}
		}
        #endregion

        #region Export Value
		private void ExportCurField()
		{
			if (EditorRuntime.CurSelectTaskNode == null)
				return;
			var editField = EditorRuntime.CurSelectTaskNode.GetEditField(m_FieldName);
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
        #endregion
    }
}
