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

		public override void _Ready()
		{
			EditorModel.OnCurSelectTaskNodeChanged += OnTaskChanged;
			BtnDelete.Pressed += OnBtnDeletePress;
			BtnMoveUp.Pressed += OnBtnMoveUpPress;
			BtnMoveDown.Pressed += OnBtnMoveDownPress;
            BtnMoveUp.Visible = false;
            BtnMoveDown.Visible = false;
            BtnDelete.Visible = false;
        }

        public override void _ExitTree()
        {
            EditorModel.OnCurSelectTaskNodeChanged -= OnTaskChanged;
        }

        private void OnTaskChanged()
		{
			// buttons
			if (EditorModel.CurSelectTaskNode == null)
			{
				BtnMoveUp.Visible = false;
				BtnMoveDown.Visible = false;
				BtnDelete.Visible = false;
			}
			else if (EditorModel.CurSelectTaskNode is TimelineNode)
			{
                BtnMoveUp.Visible = true;
                BtnMoveDown.Visible = true;
                BtnDelete.Visible = true;
            }
			// title
			if (EditorModel.CurSelectTaskNode == null)
			{
				Title.Text = "Inspector";
			}
			else
			{
				Title.Text = TaskUtils.GetTaskDisplayName(EditorModel.CurSelectTaskNode.TaskEditData.TaskType);
			}
			// fields
			FieldItemRoot.RemoveChildren();
			var selectNode = EditorModel.CurSelectTaskNode;
			if (selectNode == null)
				return;
			if (selectNode is TimelineNode timelineNode)
			{
                var fieldItem = FieldPrefab.Instantiate<InspectorFieldItem>();
                fieldItem.RebindSpecialField(InspectorFieldItem.ESpecialField.TimelineStartTime);
                FieldItemRoot.AddChild(fieldItem);
                fieldItem = FieldPrefab.Instantiate<InspectorFieldItem>();
                fieldItem.RebindSpecialField(InspectorFieldItem.ESpecialField.TimelineDuration);
                FieldItemRoot.AddChild(fieldItem);
            }
			var fields = selectNode.TaskEditData.Fields;
            for (int i = 0; i < fields.Count; i++)
			{
				var editField = fields[i];
				if (editField.FieldName == "Duration" && selectNode is TimelineNode)	// use timeline node's duration as TaskDuration's duration
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
						EditorModel.TimelineData.OnTimelineTasksChanged();
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
						EditorModel.TimelineData.OnTimelineTasksChanged();
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
                        EditorModel.TimelineData.OnTimelineTasksChanged();
						EditorModel.CurSelectTaskNode = EditorModel.TimelineData.Nodes[i + 1];
                        return;
                    }
                }
            }
        }
    }
}
