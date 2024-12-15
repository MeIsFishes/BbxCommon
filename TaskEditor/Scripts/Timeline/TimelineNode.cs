using Godot;
using System;

namespace BbxCommon
{
	public partial class TimelineNode : TaskNode
	{
        [Export]
        public BbxButton TaskButton;

        protected override void OnBind(string taskType)
        {
            TaskButton.Text = taskType;
            var editField = new TaskEditField();
            editField.FieldName = "Duration";
            editField.TypeInfo = new Internal.TaskExportTypeInfo("float");
            editField.Value = "0";
            AddEditFieldFront(editField);
        }
    }
}
