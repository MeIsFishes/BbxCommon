using Godot;
using System;

namespace BbxCommon
{
	public partial class TimelineNode : TaskNode
	{
        [Export]
        public BbxButton TaskButton;
        [Export]
        public ColorRect DurationBarRect;

        private float m_DurationBarOriginalWidth;

        private TaskTimelineEditData m_TimelineEditData = new();
        public override TaskEditData TaskEditData => m_TimelineEditData;

        protected override void OnReady()
        {
            m_DurationBarOriginalWidth = DurationBarRect.Size.X;
        }

        protected override void OnBind(string taskType)
        {
            taskType = taskType.TryRemoveStart("Task");
            TaskButton.Text = taskType;
        }
    }
}
