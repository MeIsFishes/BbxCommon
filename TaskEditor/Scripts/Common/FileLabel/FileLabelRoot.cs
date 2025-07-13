using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
    public partial class FileLabelRoot : BbxControl
    {
        [Export]
        public PackedScene FileLabelItemPrefab;
        [Export]
        public Control FileLabelItemContainer;
        [Export]
        public BbxButton MoreLabelButton;
        [Export]
        public Control MoreLabelContainer;
        [Export]
        public BbxButton CreateButton;
        [Export]
        public Control CreateContainer;
        [Export]
        public BbxButton CreateTimelineButton;
        [Export]
        public BbxButton CreateGraphNodeButton;

        public static readonly int LabelLimit = 6;

        private List<FileLabelItem> m_LabelItems = new();

        protected override void OnUiInit()
        {
            EventBus.RegisterEvent(EEvent.SaveTargetListChanged, OnSaveTargetListChanged);
            EventBus.RegisterEvent(EEvent.CurSaveTargetChanged, OnCurSaveTargetChanged);
            MoreLabelButton.Pressed += OnMoreLabelButtonPressed;
            CreateButton.Pressed += OnCreateButtonPressed;
            CreateTimelineButton.Pressed += OnCreateTimelineButtonPressed;
            CreateGraphNodeButton.Pressed += OnCreateNodeGraphButtonPressed;

            CreateContainer.Visible = false;
            MoreLabelContainer.Visible = false;
        }

        protected override void OnUiDestroy()
        {
            EventBus.UnregisterEvent(EEvent.SaveTargetListChanged, OnSaveTargetListChanged);
            EventBus.UnregisterEvent(EEvent.CurSaveTargetChanged, OnCurSaveTargetChanged);
        }

        protected override void OnUiUpdate(double deltaTime)
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                if (MoreLabelContainer.Visible && !MoreLabelContainer.GetRect().HasPoint(GetGlobalMousePosition()) &&
                    !MoreLabelButton.GetRect().HasPoint(GetGlobalMousePosition()))
                {
                    MoreLabelContainer.Visible = false;
                }
                if (CreateContainer.Visible && !CreateContainer.GetRect().HasPoint(GetGlobalMousePosition()) &&
                    !CreateButton.GetRect().HasPoint(GetGlobalMousePosition()))
                {
                    CreateContainer.Visible = false;
                }
            }
        }

        private void OnSaveTargetListChanged()
        {
            FileLabelItemContainer.RemoveChildren();
            for (int i = 0; i < EditorModel.SaveTargetList.Count && i < LabelLimit; i++)
            {
                var saveTarget = EditorModel.SaveTargetList[i];
                var item = FileLabelItemPrefab.Instantiate<FileLabelItem>();
                item.SetSaveTarget(saveTarget);
                FileLabelItemContainer.AddChild(item);
                m_LabelItems.Add(item);
            }
            // set others to MoreContainer
            if (EditorModel.SaveTargetList.Count > LabelLimit)
            {
                MoreLabelContainer.RemoveChildren();
                for (int i = LabelLimit; i < EditorModel.SaveTargetList.Count; i++)
                {
                    var saveTarget = EditorModel.SaveTargetList[i];
                    var item = FileLabelItemPrefab.Instantiate<FileLabelItem>();
                    item.SetSaveTarget(saveTarget);
                    MoreLabelContainer.AddChild(item);
                    m_LabelItems.Add(item);
                }
            }
            OnCurSaveTargetChanged();
        }

        private void OnCurSaveTargetChanged()
        {
            for (int i = 0; i < m_LabelItems.Count; i++)
            {
                m_LabelItems[i].SetSelected(EditorModel.CurSaveTarget == m_LabelItems[i].SaveTarget);
            }
        }

        private void OnMoreLabelButtonPressed()
        {
            MoreLabelContainer.Visible = !MoreLabelContainer.Visible;
        }

        private void OnCreateButtonPressed()
        {
            CreateContainer.Visible = !CreateContainer.Visible;
        }

        private void OnCreateTimelineButtonPressed()
        {
            var saveTarget = new EditorModel.TimelineSaveTargetData();
            EditorModel.SaveTargetList.Insert(0, saveTarget);
            EventBus.DispatchEvent(EEvent.SaveTargetListChanged);
            CreateContainer.Visible = false;
        }

        private void OnCreateNodeGraphButtonPressed()
        {
            var saveTarget = new EditorModel.NodeGraphSaveTargetData();
            EditorModel.SaveTargetList.Insert(0, saveTarget);
            EventBus.DispatchEvent(EEvent.SaveTargetListChanged);
            CreateContainer.Visible = false;
        }
    }
}
