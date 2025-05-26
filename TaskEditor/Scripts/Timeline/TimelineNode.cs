using Godot;
using Godot.Collections;
using System;

namespace BbxCommon
{
	public partial class TimelineNode : TaskNode
	{
        #region Common
        [Export]
        public BbxButton TaskButton;
        [Export]
        public ColorRect DurationBarRect;
        [Export]
        public Array<Control> SelectedControls;

        private float m_DurationBarOriginalX;
        private float m_DurationBarOriginalWidth;

        public TaskTimelineEditData TimelineEditData => TaskEditData as TaskTimelineEditData;

        protected override void OnReady()
        {
            EventBus.RegisterEvent(EEvent.CurSelectTaskNodeChanged, OnCurSelectNodeChanged);
            m_DurationBarOriginalX = DurationBarRect.Position.X;
            m_DurationBarOriginalWidth = DurationBarRect.Size.X;
            OnCurSelectNodeChanged();
            RefreshDurationBar();
        }

        protected override void AddInspectorButton()
        {
            AddButton("Move Up", OnBtnMoveUpPress);
            AddButton("Move Down", OnBtnMoveDownPress);
            AddButton("Delete", OnBtnDeletePress);
        }

        public override void _ExitTree()
        {
            EventBus.UnregisterEvent(EEvent.TimelineMaxTimeChanged, RefreshDurationBar);
            EventBus.UnregisterEvent(EEvent.CurSelectTaskNodeChanged, OnCurSelectNodeChanged);
        }

        protected override void OnBind(TaskEditData editData)
        {
            TimelineEditData.OnStartTimeChanged = RefreshDurationBar;
            TimelineEditData.OnDurationChanged = RefreshDurationBar;
            var taskType = TaskUtils.GetTaskDisplayName(editData.TaskType);
            TaskButton.Text = taskType;
        }

        public void RefreshDurationBar()
        {
            var maxTime = EditorModel.TimelineData.MaxTime;
            if (maxTime == 0)
                return;
            float startX = m_DurationBarOriginalX + (TimelineEditData.StartTime / maxTime) * m_DurationBarOriginalWidth;
            float endX = m_DurationBarOriginalX + ((TimelineEditData.StartTime + TimelineEditData.Duration) / maxTime) * m_DurationBarOriginalWidth;
            DurationBarRect.Position = new Vector2(startX, DurationBarRect.Position.Y);
            DurationBarRect.Size = new Vector2(endX - startX, DurationBarRect.Size.Y);
        }

        private void OnCurSelectNodeChanged()
        {
            if (EditorModel.CurSelectTaskNode == this)
            {
                for (int i = 0; i < SelectedControls.Count; i++)
                {
                    SelectedControls[i].Visible = true;
                }
            }
            else
            {
                for (int i = 0; i < SelectedControls.Count; i++)
                {
                    SelectedControls[i].Visible = false;
                }
            }
        }
        #endregion

        #region Inspector Button
        private void OnBtnDeletePress()
        {
            for (int i = 0; i < EditorModel.TimelineData.TaskDatas.Count; i++)
            {
                if (EditorModel.TimelineData.TaskDatas[i] == TaskEditData)
                {
                    EditorModel.TimelineData.TaskDatas.RemoveAt(i);
                    EventBus.DispatchEvent(EEvent.TimelineTasksChanged);
                    EditorModel.CurSelectTaskNode = null;
                    return;
                }
            }
        }

        private void OnBtnMoveUpPress()
        {
            for (int i = 0; i < EditorModel.TimelineData.TaskDatas.Count; i++)
            {
                if (EditorModel.TimelineData.TaskDatas[i] == TaskEditData)
                {
                    if (i == 0)
                        return;
                    var temp = EditorModel.TimelineData.TaskDatas[i - 1];
                    EditorModel.TimelineData.TaskDatas[i - 1] = EditorModel.TimelineData.TaskDatas[i];
                    EditorModel.TimelineData.TaskDatas[i] = temp;
                    EventBus.DispatchEvent(EEvent.TimelineTasksChanged);
                    EditorModel.CurSelectTaskNode = EditorModel.TimelineData.Nodes[i - 1];
                    return;
                }
            }
        }

        private void OnBtnMoveDownPress()
        {
            for (int i = 0; i < EditorModel.TimelineData.TaskDatas.Count; i++)
            {
                if (EditorModel.TimelineData.TaskDatas[i] == TaskEditData)
                {
                    if (i == EditorModel.TimelineData.TaskDatas.Count - 1)
                        return;
                    var temp = EditorModel.TimelineData.TaskDatas[i + 1];
                    EditorModel.TimelineData.TaskDatas[i + 1] = EditorModel.TimelineData.TaskDatas[i];
                    EditorModel.TimelineData.TaskDatas[i] = temp;
                    EventBus.DispatchEvent(EEvent.TimelineTasksChanged);
                    EditorModel.CurSelectTaskNode = EditorModel.TimelineData.Nodes[i + 1];
                    return;
                }
            }
        }
        #endregion
    }
}
