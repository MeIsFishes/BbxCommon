using Godot;
using System;

namespace BbxCommon
{
	public partial class TipsText : BbxControl
	{
        [Export]
        public Control Frame;
        [Export]
        public Control Border;
        [Export]
        public Label TextLabel;
        [Export]
        public int BorderPadding;
        [Export]
        public int LabelPadding;

        protected override void OnUiInit()
        {
            EditorModel.TipsText = this;
            this.Visible = false;
        }

        protected override void OnUiOpen()
        {
            TextLabel.Size = new Vector2(Frame.Size.X - LabelPadding * 2, TextLabel.Size.Y);
            TextLabel.Position = new Vector2(LabelPadding, LabelPadding);
        }

        public void SetText(string text)
        {
            TextLabel.Text = text;
            var height = TextLabel.GetLineCount() * TextLabel.GetLineHeight();
            Frame.Size = new Vector2(Frame.Size.X, height + LabelPadding * 2);
            Border.Size = new Vector2(Frame.Size.X - BorderPadding * 2, Frame.Size.Y - BorderPadding * 2);
            Border.Position = new Vector2(BorderPadding, BorderPadding);
        }
    }
}
