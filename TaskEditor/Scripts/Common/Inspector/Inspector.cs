using Godot;
using System;

namespace BbxCommon
{
	public partial class Inspector : BbxControl
	{
		[Export]
		public PackedScene FieldPrefab;
		[Export]
		public Container FieldItemRoot;
		[Export]
		public PackedScene ButtonPrefab;
		[Export]
		public Container ButtonItemRoot;
		[Export]
		public Label Title;

		private TaskNode m_SelectedNode;

		protected override void OnUiOpen()
		{
			EventBus.RegisterEvent(EEvent.CurSelectTaskNodeChanged, OnTaskChanged);
            ButtonItemRoot.SortChildren += RecalculateButtonSize;
        }

        protected override void OnUiClose()
        {
            EventBus.UnregisterEvent(EEvent.CurSelectTaskNodeChanged, OnTaskChanged);
            ButtonItemRoot.SortChildren -= RecalculateButtonSize;
        }

        private void OnTaskChanged()
		{
			// refresh selected node
			m_SelectedNode = EditorModel.CurSelectTaskNode;
			// buttons
			ButtonItemRoot.RemoveChildren();
			foreach (var buttonData in m_SelectedNode.InspectorButtonDatas)
			{
				var buttonItem = ButtonPrefab.Instantiate<InspectorButton>();
				buttonItem.SetData(buttonData);
                ButtonItemRoot.AddChild(buttonItem);
            }
			// title
			if (m_SelectedNode == null)
			{
				Title.Text = "Inspector";
			}
			else
			{
				Title.Text = TaskUtils.GetTaskDisplayName(m_SelectedNode.TaskEditData.TaskType);
			}
			// fields
			FieldItemRoot.RemoveChildren();
			if (m_SelectedNode == null)
				return;
			if (m_SelectedNode is TimelineNode timelineNode)
			{
                var fieldItem = FieldPrefab.Instantiate<InspectorFieldItem>();
                fieldItem.RebindSpecialField(InspectorFieldItem.ESpecialField.TimelineStartTime, m_SelectedNode);
                FieldItemRoot.AddChild(fieldItem);
                fieldItem = FieldPrefab.Instantiate<InspectorFieldItem>();
                fieldItem.RebindSpecialField(InspectorFieldItem.ESpecialField.TimelineDuration, m_SelectedNode);
                FieldItemRoot.AddChild(fieldItem);
            }
			var fields = m_SelectedNode.TaskEditData.Fields;
            for (int i = 0; i < fields.Count; i++)
			{
				var editField = fields[i];
				if (editField.FieldName == "Duration" && m_SelectedNode is TimelineNode)	// use timeline node's duration as TaskDuration's duration
					continue;
				var fieldItem = FieldPrefab.Instantiate<InspectorFieldItem>();
				FieldItemRoot.AddChild(fieldItem);
                fieldItem.RebindField(editField, m_SelectedNode);
            }
		}

		private void RecalculateButtonSize()
		{
			if (ButtonItemRoot.GetChildCount() == 0)
				return;
            var firstChild = ButtonItemRoot.GetChild<Control>(0);
			var firstY = firstChild.Position.Y;
			var lastChild = ButtonItemRoot.GetChild<Control>(ButtonItemRoot.GetChildCount() - 1);
			var lastY = lastChild.Position.Y + lastChild.Size.Y;
			float buttonSizeY = lastY - firstY;
			float rootY = GetTree().Root.Size.Y - 10;	// -10 to keep the bottom padding
            ButtonItemRoot.Position = new Vector2(ButtonItemRoot.Position.X, rootY - buttonSizeY);
			ButtonItemRoot.Size = new Vector2(ButtonItemRoot.Size.X, buttonSizeY);
			FieldItemRoot.Size = new Vector2(FieldItemRoot.Size.X, rootY - FieldItemRoot.Position.Y - buttonSizeY);
		}
    }
}
