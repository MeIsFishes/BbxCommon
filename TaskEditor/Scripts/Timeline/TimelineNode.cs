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

        private float m_DurationBarOriginalX;
        private float m_DurationBarOriginalWidth;

        private TaskTimelineEditData m_TimelineEditData = new();
        public TaskTimelineEditData TimelineEditData => m_TimelineEditData;
        public override TaskEditData TaskEditData => m_TimelineEditData;

        protected override void OnReady()
        {
            EditorModel.TimelineData.OnMaxTimeChanged += RefreshDurationBar;
            m_TimelineEditData.OnStartTimeChanged += RefreshDurationBar;
            m_TimelineEditData.OnDurationChanged += RefreshDurationBar;
            m_DurationBarOriginalX = DurationBarRect.Position.X;
            m_DurationBarOriginalWidth = DurationBarRect.Size.X;
            RefreshDurationBar();
        }

        protected override void OnBind(string taskType)
        {
            taskType = taskType.TryRemoveStart("Task");
            TaskButton.Text = taskType;
        }

        public void RefreshDurationBar()
        {
            var maxTime = EditorModel.TimelineData.MaxTime;
            if (maxTime == 0)
                return;
            float startX = m_DurationBarOriginalX + (m_TimelineEditData.StartTime / maxTime) * m_DurationBarOriginalWidth;
            float endX = m_DurationBarOriginalX + ((m_TimelineEditData.StartTime + m_TimelineEditData.Duration) / maxTime) * m_DurationBarOriginalWidth;
            DurationBarRect.Position = new Vector2(startX, DurationBarRect.Position.Y);
            DurationBarRect.Size = new Vector2(endX - startX, DurationBarRect.Size.Y);
        }
    }
}
