using BbxCommon.Internal;
using Godot;
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

        private TaskSelector m_TaskSelector;

        public override void _Ready()
        {
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
        }
    }
}
