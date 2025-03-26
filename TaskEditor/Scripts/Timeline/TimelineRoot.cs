using BbxCommon.Internal;
using Godot;
using Godot.Collections;
using System;

namespace BbxCommon
{
	public partial class TimelineRoot : Node, ITaskSelectorTarget
	{
        [Export]
        public PackedScene TaskSelectorPrefab;
        [Export]
        public PackedScene TaskNodePrefab;
        [Export]
        public Control TaskNodeRoot;
		[Export]
		public Button NewTaskButton;
        [Export]
        public Array<Label> TimeBarLabel;

        private TaskSelector m_TaskSelector;

        public override void _Ready()
        {
            EditorModel.TimelineData.OnTaskStartTimeOrDurationChanged += OnTaskStartTimeAndDurationChanged;
            NewTaskButton.Pressed += OnNewTaskButtonClick;
        }

        private void OnNewTaskButtonClick()
        {
            if (m_TaskSelector == null)
            {
                m_TaskSelector = TaskSelectorPrefab.Instantiate<TaskSelector>();
                m_TaskSelector.SetTarget(this);
                this.AddChild(m_TaskSelector);
            }
            m_TaskSelector.Visible = true;
        }

        void ITaskSelectorTarget.SelectTask(TaskExportInfo taskInfo)
        {
            var node = TaskNodePrefab.Instantiate<TimelineNode>();
            node.BindTask(taskInfo.TaskTypeName);
            TaskNodeRoot.AddChild(node);
            TaskNodeRoot.CustomMinimumSize = new Vector2(node.CustomMinimumSize.X,
                TaskNodeRoot.CustomMinimumSize.Y + node.CustomMinimumSize.Y);
            EditorModel.TimelineData.Nodes.Add(node);
        }

        private void OnTaskStartTimeAndDurationChanged()
        {
            if (TimeBarLabel.Count < 2)
                return;
            float maxTime = 0;
            for (int i = 0; i < EditorModel.TimelineData.Nodes.Count; i++)
            {
                var node = EditorModel.TimelineData.Nodes[i];
                if (node.TimelineEditData.StartTime + node.TimelineEditData.Duration > maxTime)
                    maxTime = node.TimelineEditData.StartTime + node.TimelineEditData.Duration;
            }
            EditorModel.TimelineData.MaxTime = maxTime;
            float timeStep = maxTime / (TimeBarLabel.Count - 1);
            for (int i = 0; i < TimeBarLabel.Count; i++)
            {
                TimeBarLabel[i].Text = (timeStep * i).ToString("0.00");
            }
        }
    }
}
