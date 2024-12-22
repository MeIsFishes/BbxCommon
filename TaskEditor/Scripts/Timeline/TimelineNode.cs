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
            var fieldStartTime = new TaskEditField();
            fieldStartTime.FieldName = "StartTime";
            fieldStartTime.TypeInfo = new Internal.TaskExportTypeInfo("float");
            fieldStartTime.Value = "0";
            InsertEditField(0, fieldStartTime);
            var fieldDuration = new TaskEditField();
            fieldDuration.FieldName = "Duration";
            fieldDuration.TypeInfo = new Internal.TaskExportTypeInfo("float");
            fieldDuration.Value = "0";
            InsertEditField(1, fieldDuration);
        }
    }
}
