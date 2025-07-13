using Godot;
using System;

namespace BbxCommon
{
	public partial class FileLabelItem : BbxControl
	{
		[Export]
		public Control Normal;
		[Export]
		public Control Selected;
		[Export]
		public BbxButton SelectButton;
		[Export]
		public BbxButton CloseButton;

		public EditorModel.SaveTargetBase SaveTarget;

        public void SetSaveTarget(EditorModel.SaveTargetBase saveTarget)
		{
			SaveTarget = saveTarget;
			if (saveTarget.FilePath.IsNullOrEmpty())
				SelectButton.Text = "NewCreateTask";
			else
				SelectButton.Text = FileApi.GetLastDirectoryOrFileOfPath(saveTarget.FilePath);

			SelectButton.Pressed = null;
			SelectButton.Pressed += () =>
			{
				EditorModel.CurSaveTarget = SaveTarget;
			};

			CloseButton.Pressed = null;
			CloseButton.Pressed += () =>
			{
				if (EditorModel.CurSaveTarget == SaveTarget)
				{
					if (EditorModel.SaveTargetList.Count > 1)
					{
						EditorModel.CurSaveTarget = EditorModel.SaveTargetList[0];
					}
					else
					{
						EditorModel.CurSaveTarget = null;
					}
				}
				EditorModel.SaveTargetList.Remove(SaveTarget);
				EventBus.DispatchEvent(EEvent.SaveTargetListChanged);
			};
        }

		public void SetSelected(bool selected)
        {
            Normal.Visible = !selected;
            Selected.Visible = selected;
        }
    }
}
