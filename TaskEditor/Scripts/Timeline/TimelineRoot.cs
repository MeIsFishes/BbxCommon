using BbxCommon.Internal;
using Godot;
using Godot.Collections;
using System.Collections.Generic;

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

        public List<TimelineNode> Nodes = new();

        protected override void OnUiInit()
        {
            EditorModel.TimelineRoot = this;
            EventBus.RegisterEvent(EEvent.EditorDataStoreRefresh, OnEditorDataStoreRefresh);
            EventBus.RegisterEvent(EEvent.ReloadEditingTaskData, OnEditorDataStoreRefresh);
            EventBus.RegisterEvent(EEvent.TimelineNodeStartTimeOrDurationChanged, OnTaskStartTimeAndDurationChanged);
            EventBus.RegisterEvent(EEvent.TimelineTasksChanged, OnTimelineTasksChanged);
            NewTaskButton.Pressed += OnNewTaskButtonClick;
        }

        protected override void OnUiDestroy()
        {
            EventBus.UnregisterEvent(EEvent.EditorDataStoreRefresh, OnEditorDataStoreRefresh);
            EventBus.UnregisterEvent(EEvent.ReloadEditingTaskData, OnEditorDataStoreRefresh);
            EventBus.UnregisterEvent(EEvent.TimelineNodeStartTimeOrDurationChanged, OnTaskStartTimeAndDurationChanged);
            EventBus.UnregisterEvent(EEvent.TimelineTasksChanged, OnTimelineTasksChanged);
        }

        private void OnNewTaskButtonClick()
        {
            EditorModel.TaskSelector.OpenWithTags((taskInfo) =>
            {
                var editData = TaskUtils.TaskExportInfoToTimelineEditData(taskInfo);
                EditorModel.TimelineSaveTarget.TaskDatas.Add(editData);
                EventBus.DispatchEvent(EEvent.TimelineTasksChanged);
            },
            TaskExportCrossVariable.TaskTagAction);
        }

        private void OnEditorDataStoreRefresh()
        {
            if (EditorModel.CurSaveTarget.IsTimeline)
            {
                OnTimelineTasksChanged();
            }
        }

        private void OnTimelineTasksChanged()
        {
            TaskNodeRoot.RemoveChildren();
            Nodes.Clear();
            for (int i = 0; i < EditorModel.TimelineSaveTarget.TaskDatas.Count; i++)
            {
                var node = TaskNodePrefab.Instantiate<TimelineNode>();
                TaskNodeRoot.AddChild(node);
                node.BindTask(EditorModel.TimelineSaveTarget.TaskDatas[i]);
                Nodes.Add(node);
            }
        }

        private void OnTaskStartTimeAndDurationChanged()
        {
            if (TimeBarLabel.Count < 2)
                return;
            float maxTime = 0;
            for (int i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];
                if (node.TimelineEditData.StartTime + node.TimelineEditData.Duration > maxTime)
                    maxTime = node.TimelineEditData.StartTime + node.TimelineEditData.Duration;
            }
            EditorModel.TimelineSaveTarget.MaxTime = maxTime;
            float timeStep = maxTime / (TimeBarLabel.Count - 1);
            for (int i = 0; i < TimeBarLabel.Count; i++)
            {
                TimeBarLabel[i].Text = (timeStep * i).ToString("0.00");
            }
        }
    }
}
