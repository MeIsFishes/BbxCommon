using BbxCommon.Internal;
using Godot;
using Godot.Collections;
using System;

namespace BbxCommon
{
	public partial class TimelineNode : TaskNode
	{
        #region Common
        [Export]
        public ColorRect DurationBarRect;
        [Export]
        public Array<Control> SelectedControls;
        [Export]
        public BbxButton ConditionButton;
        [Export]
        public PackedScene ConditionContainerPrefab;
        [Export]
        public VBoxContainer ConditionContainerRoot;

        private float m_DurationBarOriginalX;
        private float m_DurationBarOriginalWidth;

        public TaskTimelineEditData TimelineEditData => TaskEditData as TaskTimelineEditData;

        protected override void OnTaskUiOpen()
        {
            EventBus.RegisterEvent(EEvent.TimelineMaxTimeChanged, RefreshDurationBar);
            m_DurationBarOriginalX = DurationBarRect.Position.X;
            m_DurationBarOriginalWidth = DurationBarRect.Size.X;
            RefreshDurationBar();
            InitConditionContainer();

            ConditionContainerRoot.SortChildren += RecalculateSize;
            ConditionButton.Pressed += OnConditionButton;
        }

        protected override void OnTaskUiClose()
        {
            EventBus.UnregisterEvent(EEvent.TimelineMaxTimeChanged, RefreshDurationBar);

            ConditionContainerRoot.SortChildren -= RecalculateSize;
            ConditionButton.Pressed -= OnConditionButton;
        }

        protected override void AddInspectorButton()
        {
            AddButton("Move Up", OnBtnMoveUpPress);
            AddButton("Move Down", OnBtnMoveDownPress);
            AddButton("Delete", OnBtnDeletePress);
        }

        protected override void OnBindTask(TaskEditData editData)
        {
            TimelineEditData.OnStartTimeChanged = RefreshDurationBar;
            TimelineEditData.OnDurationChanged = RefreshDurationBar;
            RefreshConditionContainer();
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

        public override void OnTaskSelected(bool selected)
        {
            if (selected)
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

        private void RecalculateSize()
        {
            var size = this.GetSizeIncludeChildren();
            this.CustomMinimumSize = size;
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

        #region Conditions
        private TimelineConditionContainer m_EnterConditionContainer;
        private TimelineConditionContainer m_ConditionContainer;
        private TimelineConditionContainer m_ExitConditionContainer;

        private void InitConditionContainer()
        {
            m_EnterConditionContainer = ConditionContainerPrefab.Instantiate<TimelineConditionContainer>();
            ConditionContainerRoot.AddChild(m_EnterConditionContainer);
            m_EnterConditionContainer.SetConditionType(EConditionType.EnterCondition);
            m_EnterConditionContainer.AddCreateButtonCallback(() =>
            {
                EditorModel.TaskSelector.OpenWithTags((taskInfo) =>
                {
                    var timelineData = this.TaskEditData as TaskTimelineEditData;
                    var editData = TaskUtils.ExportInfoToTimelineEditData(taskInfo);
                    timelineData.EnterConditions.Add(editData);
                    m_EnterConditionContainer.RefreshConditionList(timelineData.EnterConditions);
                },
                TaskExportCrossVariable.TaskTagCondition);
            });

            m_ConditionContainer = ConditionContainerPrefab.Instantiate<TimelineConditionContainer>();
            ConditionContainerRoot.AddChild(m_ConditionContainer);
            m_ConditionContainer.SetConditionType(EConditionType.Condition);
            m_ConditionContainer.AddCreateButtonCallback(() =>
            {
                EditorModel.TaskSelector.OpenWithTags((taskInfo) =>
                {
                    var timelineData = this.TaskEditData as TaskTimelineEditData;
                    var editData = TaskUtils.ExportInfoToTimelineEditData(taskInfo);
                    timelineData.Conditions.Add(editData);
                    m_ConditionContainer.RefreshConditionList(timelineData.Conditions);
                },
                TaskExportCrossVariable.TaskTagCondition);
            });

            m_ExitConditionContainer = ConditionContainerPrefab.Instantiate<TimelineConditionContainer>();
            ConditionContainerRoot.AddChild(m_ExitConditionContainer);
            m_ExitConditionContainer.SetConditionType(EConditionType.ExitCondition);
            m_ExitConditionContainer.AddCreateButtonCallback(() =>
            {
                EditorModel.TaskSelector.OpenWithTags((taskInfo) =>
                {
                    var timelineData = this.TaskEditData as TaskTimelineEditData;
                    var editData = TaskUtils.ExportInfoToTimelineEditData(taskInfo);
                    timelineData.ExitConditions.Add(editData);
                    m_ExitConditionContainer.RefreshConditionList(timelineData.ExitConditions);
                },
                TaskExportCrossVariable.TaskTagCondition);
            });
        }

        private void RefreshConditionContainer()
        {
            ConditionContainerRoot.Visible = (TaskEditData as TaskTimelineEditData).ExpandCondition;
            if (ConditionContainerRoot.Visible == true)
            {
                m_EnterConditionContainer.RefreshConditionList(TimelineEditData.EnterConditions);
                m_ConditionContainer.RefreshConditionList(TimelineEditData.Conditions);
                m_ExitConditionContainer.RefreshConditionList(TimelineEditData.ExitConditions);
            }
            RecalculateSize();
        }

        private void OnConditionButton()
        {
            (TaskEditData as TaskTimelineEditData).ExpandCondition = !ConditionContainerRoot.Visible;
            RefreshConditionContainer();
        }
        #endregion
    }
}
