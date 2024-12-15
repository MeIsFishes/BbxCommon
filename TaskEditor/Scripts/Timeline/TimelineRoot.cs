using Godot;
using System;

namespace BbxCommon
{
	public partial class TimelineRoot : Node
	{
        [Export]
        public PackedScene TaskNodePrefab;
        [Export]
        public Control TaskNodeRoot;
		[Export]
		public Button NewTaskButton;

        public override void _Ready()
        {
            NewTaskButton.Pressed += OnNewTaskButtonClick;
        }

        private void OnNewTaskButtonClick()
        {
            var node = TaskNodePrefab.Instantiate<TimelineNode>();
            node.BindTask("TaskNodeDebugLog");
            TaskNodeRoot.AddChild(node);
            TaskNodeRoot.CustomMinimumSize = new Vector2(node.CustomMinimumSize.X,
                TaskNodeRoot.CustomMinimumSize.Y + node.CustomMinimumSize.Y);
        }
    }
}
