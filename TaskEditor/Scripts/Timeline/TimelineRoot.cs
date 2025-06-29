using BbxCommon.Internal;
using Godot;
using Godot.Collections;
using System;

namespace BbxCommon
{
	public partial class TimelineRoot : BbxControl
	{
        [Export]
        public PackedScene TaskNodePrefab;
        [Export]
        public Control TaskNodeRoot;
		[Export]
		public BbxButton NewTaskButton;
        [Export]
        public Array<Label> TimeBarLabel;

        protected override void OnUiInit()
        {
            EventBus.RegisterEvent(EEvent.TimelineNodeStartTimeOrDurationChanged, OnTaskStartTimeAndDurationChanged);
            EventBus.RegisterEvent(EEvent.TimelineTasksChanged, OnTimelineTasksChanged);
            NewTaskButton.Pressed += OnNewTaskButtonClick;
        }

        protected override void OnUiDestroy()
        {
            EventBus.UnregisterEvent(EEvent.TimelineNodeStartTimeOrDurationChanged, OnTaskStartTimeAndDurationChanged);
            EventBus.UnregisterEvent(EEvent.TimelineTasksChanged, OnTimelineTasksChanged);
        }

        private void OnNewTaskButtonClick()
        {
            EditorModel.TaskSelector.OpenWithTags((taskInfo) =>
            {
                var editData = TaskUtils.ExportInfoToTimelineEditData(taskInfo);
                EditorModel.TimelineData.TaskDatas.Add(editData);
                EventBus.DispatchEvent(EEvent.TimelineTasksChanged);
            },
            TaskExportCrossVariable.TaskTagAction);
        }

        private void OnTimelineTasksChanged()
        {
            TaskNodeRoot.RemoveChildren();
            EditorModel.TimelineData.Nodes.Clear();
            for (int i = 0; i < EditorModel.TimelineData.TaskDatas.Count; i++)
            {
                var node = TaskNodePrefab.Instantiate<TimelineNode>();
                TaskNodeRoot.AddChild(node);
                node.BindTask(EditorModel.TimelineData.TaskDatas[i]);
                EditorModel.TimelineData.Nodes.Add(node);
            }
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
