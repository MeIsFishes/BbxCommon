using Godot;
using System;

namespace BbxCommon
{
	public partial class InspectorButton : BbxControl
	{
		[Export]
		public Button Button;

		public void SetData(TaskNode.ButtonData buttonData)
		{
			Button.Text = buttonData.Name;
			Button.Pressed += buttonData.Callback;
		}
	}
}
