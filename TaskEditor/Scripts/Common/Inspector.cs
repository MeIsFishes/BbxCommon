using Godot;
using System;

namespace BbxCommon
{
	public partial class Inspector : Node
	{
		[Export]
		public PackedScene FieldPrefab;
		[Export]
		public Container FieldItemRoot;
		[Export]
		public Label Title;
		[Export]
		public BbxButton BtnMoveUp;
		[Export]
		public BbxButton BtnMoveDown;
		[Export]
		public BbxButton BtnDelete;

		private TaskNode m_SelectedNode;

		public override void _Ready()
		{
			EventBus.RegisterEvent(EEvent.CurSelectTaskNodeChanged, OnTaskChanged);
			BtnDelete.Pressed += OnBtnDeletePress;
			BtnMoveUp.Pressed += OnBtnMoveUpPress;
			BtnMoveDown.Pressed += OnBtnMoveDownPress;
            BtnMoveUp.Visible = false;
            BtnMoveDown.Visible = false;
            BtnDelete.Visible = false;
        }

        public override void _ExitTree()
        {
            EventBus.UnregisterEvent(EEvent.CurSelectTaskNodeChanged, OnTaskChanged);
        }

        private void OnTaskChanged()
		{
			// refresh selected node
			ExportAllFields();
			m_SelectedNode = EditorModel.CurSelectTaskNode;
			// buttons
			if (m_SelectedNode == null)
			{
				BtnMoveUp.Visible = false;
				BtnMoveDown.Visible = false;
				BtnDelete.Visible = false;
			}
			else if (m_SelectedNode is TimelineNode)
			{
                BtnMoveUp.Visible = true;
                BtnMoveDown.Visible = true;
                BtnDelete.Visible = true;
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
                fieldItem.RebindField(editField);
            }
		}

		private void OnBtnDeletePress()
		{
			if (EditorModel.CurSelectTaskNode is TimelineNode)
			{
				for (int i = 0; i < EditorModel.TimelineData.TaskDatas.Count; i++)
				{
					if (EditorModel.CurSelectTaskNode.TaskEditData == EditorModel.TimelineData.TaskDatas[i])
					{
						EditorModel.TimelineData.TaskDatas.RemoveAt(i);
						EventBus.DispatchEvent(EEvent.TimelineTasksChanged);
						EditorModel.CurSelectTaskNode = null;
						return;
					}
				}
			}
		}

		private void OnBtnMoveUpPress()
		{
			if (EditorModel.CurSelectTaskNode is TimelineNode)
			{
				for (int i = 0; i < EditorModel.TimelineData.TaskDatas.Count; i++)
				{
					if (EditorModel.CurSelectTaskNode.TaskEditData == EditorModel.TimelineData.TaskDatas[i])
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
		}

        private void OnBtnMoveDownPress()
        {
            if (EditorModel.CurSelectTaskNode is TimelineNode)
            {
                for (int i = 0; i < EditorModel.TimelineData.TaskDatas.Count; i++)
                {
                    if (EditorModel.CurSelectTaskNode.TaskEditData == EditorModel.TimelineData.TaskDatas[i])
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
        }

		private void ExportAllFields()
		{
			if (m_SelectedNode == null)
				return;
			foreach (var field in FieldItemRoot.GetChildren<InspectorFieldItem>())
			{
				field.ExportCurField(m_SelectedNode);
			}
		}
    }
}
