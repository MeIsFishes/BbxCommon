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

		public override void _Ready()
		{
			EditorModel.OnCurSelectTaskNodeChanged += OnTaskChanged;
		}

        public override void _ExitTree()
        {
            EditorModel.OnCurSelectTaskNodeChanged -= OnTaskChanged;
        }

        private void OnTaskChanged()
		{
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
	}
}
