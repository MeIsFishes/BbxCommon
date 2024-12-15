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
			EditorRuntime.OnCurSelectTaskNodeChanged += OnTaskChanged;
		}

		private void OnTaskChanged()
		{
			FieldItemRoot.RemoveChildren();
			var selectNode = EditorRuntime.CurSelectTaskNode;
			if (selectNode == null)
				return;
			for (int i = 0; i < selectNode.Fields.Count; i++)
			{
				var editField = selectNode.Fields[i];
				var fieldItem = FieldPrefab.Instantiate<InspectorFieldItem>();
				fieldItem.RebindField(editField);
				FieldItemRoot.AddChild(fieldItem);
			}
		}
	}
}
