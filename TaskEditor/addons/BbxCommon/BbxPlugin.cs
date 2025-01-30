#if TOOLS
using Godot;
using System;

namespace BbxCommon
{
	[Tool]
	public partial class BbxPlugin : EditorPlugin
	{
		public override void _EnterTree()
		{
			var script = GD.Load<Script>("res://addons/BbxCommon/BbxButton.cs");
			AddCustomType("BbxButton", "Button", script, null);
		}

		public override void _ExitTree()
		{
			
		}
	}
}
#endif
